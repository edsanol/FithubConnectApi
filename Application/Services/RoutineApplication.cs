using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Utilities.Static;

namespace Application.Services
{
    public class RoutineApplication : IRoutineApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtHandler _jwtHandler;
        private readonly DbFithubContext _context;

        public RoutineApplication(IJwtHandler jwtHandler, DbFithubContext context, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _jwtHandler = jwtHandler;
            _context = context;
        }

        public async Task<BaseResponse<bool>> CreateExercise(NewExerciseRequestDto createExerciseRequestDto)
        {
            var response = new BaseResponse<bool>();
            string role = _jwtHandler.GetRoleFromToken();
            if (role != "gimnasio")
            {
                response.IsSuccess = false;
                response.Message = "No autorizado";
                return response;
            }
            var gymID = _jwtHandler.ExtractIdFromToken();

            try
            {
                var exercise = new Exercises
                {
                    ExerciseTitle = createExerciseRequestDto.Title,
                    ExerciseDescription = createExerciseRequestDto.Description,
                    Duration = createExerciseRequestDto.Duration,
                    VideoURL = createExerciseRequestDto.VideoURL,
                    ImageURL = createExerciseRequestDto.ImageURL,
                    IdGym = gymID,
                    IdMuscleGroup = createExerciseRequestDto.IdMuscleGroup ?? null,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsActive = true
                };
                var exerciseCreated = await _unitOfWork.ExerciseRepository.CreateExercise(exercise);
                if (!exerciseCreated)
                {
                    response.IsSuccess = false;
                    response.Message = "No se pudo crear el ejercicio.";
                    return response;
                }
                response.IsSuccess = true;
                response.Message = "El ejercicio fue creado exitosamente.";
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> CreateRoutine(CreateRoutineRequestDto createRoutineRequestDto)
        {
            var response = new BaseResponse<bool>();

            string role = _jwtHandler.GetRoleFromToken();

            if (role != "gimnasio")
            {
                response.IsSuccess = false;
                response.Message = "No autorizado";
                return response;
            }

            var gymID = _jwtHandler.ExtractIdFromToken();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (string.IsNullOrEmpty(createRoutineRequestDto.Title) ||
                    string.IsNullOrEmpty(createRoutineRequestDto.Description) ||
                    createRoutineRequestDto.IdMuscleGroup == 0 ||
                    createRoutineRequestDto.Exercises.Count == 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Todos los campos son requeridos.";
                    return response;
                }

                var routine = new Routines
                {
                    Title = createRoutineRequestDto.Title,
                    Description = createRoutineRequestDto.Description,
                    IdMuscleGroup = createRoutineRequestDto.IdMuscleGroup,
                    ImageURL = createRoutineRequestDto.ImageURL,
                    IdGym = gymID,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsActive = true
                };

                var routineCreated = await _unitOfWork.RoutineRepository.CreateRoutine(routine);
                if (!routineCreated)
                {
                    throw new Exception("No se pudo crear la rutina.");
                }

                foreach (var exerciseDto in createRoutineRequestDto.Exercises)
                {
                    long exerciseId;

                    if (exerciseDto.IdExercise.HasValue && exerciseDto.IdExercise > 0 && exerciseDto.NewExercise == null)
                    {
                        var existingExercise = await _unitOfWork.ExerciseRepository.GetExerciseById(exerciseDto.IdExercise.Value);
                        if (existingExercise == null || existingExercise.IdGym != gymID)
                        {
                            throw new Exception($"El ejercicio con ID {exerciseDto.IdExercise} no existe o no pertenece a este gimnasio.");
                        }
                        exerciseId = existingExercise.ExerciseId;
                    }
                    else if (exerciseDto.NewExercise != null && exerciseDto.IdExercise == null)
                    {
                        var newExercise = new Exercises
                        {
                            ExerciseTitle = exerciseDto.NewExercise.Title,
                            ExerciseDescription = exerciseDto.NewExercise.Description,
                            Duration = exerciseDto.NewExercise.Duration,
                            VideoURL = exerciseDto.NewExercise.VideoURL,
                            ImageURL = exerciseDto.NewExercise.ImageURL,
                            IdGym = gymID,
                            IdMuscleGroup = exerciseDto.NewExercise.IdMuscleGroup ?? null,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            IsActive = true
                        };

                        var exerciseCreated = await _unitOfWork.ExerciseRepository.CreateExercise(newExercise);
                        if (!exerciseCreated)
                        {
                            throw new Exception($"No se pudo crear el ejercicio '{exerciseDto.NewExercise.Title}'.");
                        }

                        exerciseId = newExercise.ExerciseId;
                    }
                    else
                    {
                        throw new Exception("Debe proporcionar un ejercicio existente o los datos para crear uno nuevo.");
                    }

                    var routineExercise = new RoutineExercises
                    {
                        IdRoutine = routine.RoutineId,
                        IdExercise = exerciseId,
                        Order = createRoutineRequestDto.Exercises.IndexOf(exerciseDto) + 1,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    var routineExerciseCreated = await _unitOfWork.RoutineExerciseRepository.CreateRoutineExercise(routineExercise);
                    if (!routineExerciseCreated)
                    {
                        throw new Exception($"No se pudo asociar el ejercicio con ID {exerciseId} a la rutina.");
                    }

                    foreach (var setDto in exerciseDto.Sets)
                    {
                        var routineExerciseSet = new RoutineExerciseSets
                        {
                            IdRoutineExercise = routineExercise.RoutineExerciseId,
                            SetNumber = setDto.SetNumber,
                            Reps = setDto.Reps,
                            Weight = setDto.Weight,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };

                        var setCreated = await _unitOfWork.RoutineExerciseSetsRepository.CreateRoutineExerciseSets(routineExerciseSet);
                        if (!setCreated)
                        {
                            throw new Exception($"No se pudo crear el set número {setDto.SetNumber} para el ejercicio con ID {exerciseId}.");
                        }
                    }
                }

                var athletesByChannel = await _unitOfWork.ChannelUsersRepository.GetAllAthleteIdsByChannel(createRoutineRequestDto.IdChannel);

                var existingAssignments = await _unitOfWork.AthleteRoutineRepository.GetAssignmentsByRoutineAndAthletes(
                    routine.RoutineId, athletesByChannel
                );

                var existingAssignmentsSet = new HashSet<(int athleteId, long routineId)>(
                    existingAssignments.Select(a => (a.IdAthlete, a.IdRoutine))
                );

                var athleteRoutines = new List<AthleteRoutines>();

                foreach (var athleteId in athletesByChannel)
                {
                    if (existingAssignmentsSet.Contains((athleteId, routine.RoutineId)))
                    {
                        continue;
                    }

                    var athleteRoutine = new AthleteRoutines
                    {
                        IdAthlete = athleteId,
                        IdRoutine = routine.RoutineId,
                        Status = "Active",
                        StartDate = createRoutineRequestDto.StartDate,
                        EndDate = createRoutineRequestDto.EndDate,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    athleteRoutines.Add(athleteRoutine);
                }

                if (athleteRoutines.Any())
                {
                    var athleteRoutineCreated = await _unitOfWork.AthleteRoutineRepository.CreateAthleteRoutine(athleteRoutines);
                    if (!athleteRoutineCreated)
                    {
                        throw new Exception("No se pudieron crear las asignaciones de rutina para los atletas.");
                    }
                }

                await transaction.CommitAsync();

                response.IsSuccess = true;
                response.Message = "La rutina fue creada exitosamente.";
                response.Data = true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> DeleteExercise(long exerciseId)
        {
            var response = new BaseResponse<bool>();
            string role = _jwtHandler.GetRoleFromToken();
            if (role != "gimnasio")
            {
                response.IsSuccess = false;
                response.Message = "No autorizado";
                return response;
            }
            var gymID = _jwtHandler.ExtractIdFromToken();

            try
            {
                var exerciseDeleted = await _unitOfWork.ExerciseRepository.DeleteExercise(exerciseId);
                if (!exerciseDeleted)
                {
                    response.IsSuccess = false;
                    response.Message = "No se pudo eliminar el ejercicio.";
                    return response;
                }

                response.IsSuccess = true;
                response.Message = "El ejercicio fue eliminado exitosamente.";
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<BaseEntityResponse<ExercisesResponseDto>>> GetExercisesList(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<BaseEntityResponse<ExercisesResponseDto>>();
            string role = _jwtHandler.GetRoleFromToken();
            if (role != "gimnasio")
            {
                response.IsSuccess = false;
                response.Message = "No autorizado";
                return response;
            }

            var gymID = _jwtHandler.ExtractIdFromToken();

            try
            {
                var exercises = await _unitOfWork.ExerciseRepository.GetExercisesListByGymId(filters, gymID);

                if (exercises.Items == null)
                {
                    response.IsSuccess = false;
                    response.Message = "No se encontraron ejercicios.";
                    return response;
                }

                var exercisesResponse = new BaseEntityResponse<ExercisesResponseDto>
                {
                    TotalRecords = exercises.TotalRecords,
                    Items = exercises.Items.Select(exercise => new ExercisesResponseDto
                    {
                        ExerciseId = exercise.ExerciseId,
                        ExerciseTitle = exercise.ExerciseTitle,
                        ExerciseDescription = exercise.ExerciseDescription,
                        Duration = exercise.Duration,
                        VideoURL = exercise.VideoURL,
                        ImageURL = exercise.ImageURL,
                        IdMuscleGroup = exercise.IdMuscleGroup,
                        MuscleGroupName = exercise.IdMuscleGroupNavigation != null ? exercise.IdMuscleGroupNavigation.MuscleGroupName : null
                    }).ToList()
                };

                response.IsSuccess = true;
                response.Data = exercisesResponse;
                response.Message = "Lista de ejercicios obtenida exitosamente.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<BaseEntityResponse<RoutinesResponseDto>>> GetRoutinesByAthleteIdList(BaseFiltersRequest filters, int athleteId)
        {
            var response = new BaseResponse<BaseEntityResponse<RoutinesResponseDto>>();
            string role = _jwtHandler.GetRoleFromToken();
            var userId = _jwtHandler.ExtractIdFromToken();

            if (role == "deportista")
            {
                athleteId = userId;
            }
            else if (role == "gimnasio" && athleteId > 0)
            {
                bool hasAthlete = await _unitOfWork.GymRepository.HasAthleteByAthleteID(userId, athleteId);
                if (!hasAthlete)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Message = "No autorizado";
                return response;
            }

            try
            {
                var routines = await _unitOfWork.RoutineRepository.GetRoutinesByAthleteIdList(filters, athleteId);

                var routinesResponse = new BaseEntityResponse<RoutinesResponseDto>
                {
                    TotalRecords = routines.TotalRecords,
                    Items = routines.Items.Select(routine => new RoutinesResponseDto
                    {
                        RoutineId = routine.RoutineId,
                        Title = routine.Title,
                        Description = routine.Description,
                        IdMuscleGroup = routine.IdMuscleGroup,
                        MuscleGroupName = routine.IdMuscleGroupNavigation.MuscleGroupName,
                        ImageURL = routine.ImageURL,
                        IsActive = routine.IsActive,
                        Exercises = routine.RoutineExercises.Select(exercise => new RoutineExerciseResponseDto
                        {
                            RoutineExerciseId = exercise.RoutineExerciseId,
                            IdExercise = exercise.IdExercise,
                            ExerciseTitle = exercise.IdExerciseNavigation.ExerciseTitle,
                            ExerciseDescription = exercise.IdExerciseNavigation.ExerciseDescription,
                            Duration = exercise.IdExerciseNavigation.Duration,
                            VideoURL = exercise.IdExerciseNavigation.VideoURL,
                            ImageURL = exercise.IdExerciseNavigation.ImageURL,
                            RoutineExerciseSets = exercise.RoutineExerciseSets.Select(set => new RoutineExerciseSetsResponseDto
                            {
                                RoutineExerciseSetId = set.RoutineExerciseSetId,
                                SetNumber = set.SetNumber,
                                Reps = set.Reps,
                                Weight = set.Weight
                            }).ToList()
                        }).ToList()
                    }).ToList()
                };

                response.IsSuccess = true;
                response.Data = routinesResponse;
                response.Message = "Lista de rutinas obtenida exitosamente.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<BaseEntityResponse<RoutinesResponseDto>>> GetRoutinesList(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<BaseEntityResponse<RoutinesResponseDto>>();
            string role = _jwtHandler.GetRoleFromToken();
            if (role != "gimnasio")
            {
                response.IsSuccess = false;
                response.Message = "No autorizado";
                return response;
            }

            var gymID = _jwtHandler.ExtractIdFromToken();

            try
            {
                var routines = await _unitOfWork.RoutineRepository.GetRoutinesListByGymId(filters, gymID);

                var routinesResponse = new BaseEntityResponse<RoutinesResponseDto>()
                {
                    TotalRecords = routines.TotalRecords,
                    Items = routines.Items.Select(routine => new RoutinesResponseDto
                    {
                        RoutineId = routine.RoutineId,
                        Title = routine.Title,
                        Description = routine.Description,
                        IdMuscleGroup = routine.IdMuscleGroup,
                        MuscleGroupName = routine.IdMuscleGroupNavigation.MuscleGroupName,
                        ImageURL = routine.ImageURL,
                        IsActive = routine.IsActive,
                        Exercises = routine.RoutineExercises.Select(exercise => new RoutineExerciseResponseDto
                        {
                            RoutineExerciseId = exercise.RoutineExerciseId,
                            IdExercise = exercise.IdExercise,
                            ExerciseTitle = exercise.IdExerciseNavigation.ExerciseTitle,
                            ExerciseDescription = exercise.IdExerciseNavigation.ExerciseDescription,
                            Duration = exercise.IdExerciseNavigation.Duration,
                            VideoURL = exercise.IdExerciseNavigation.VideoURL,
                            ImageURL = exercise.IdExerciseNavigation.ImageURL,
                            RoutineExerciseSets = exercise.RoutineExerciseSets.Select(set => new RoutineExerciseSetsResponseDto
                            {
                                RoutineExerciseSetId = set.RoutineExerciseSetId,
                                SetNumber = set.SetNumber,
                                Reps = set.Reps,
                                Weight = set.Weight
                            }).ToList()
                        }).ToList(),
                        AthleteRoutines = routine.AthleteRoutines.Select(athleteRoutine => new AthleteRoutinesResponseDto
                        {
                            IdAthlete = athleteRoutine.IdAthlete,
                            NameAthlete = athleteRoutine.IdAthleteNavigation.AthleteName,
                            LastNameAthlete = athleteRoutine.IdAthleteNavigation.AthleteLastName,
                            EmailAthlete = athleteRoutine.IdAthleteNavigation.Email,
                            Status = athleteRoutine.Status,
                            StartDate = athleteRoutine.StartDate,
                            EndDate = athleteRoutine.EndDate
                        }).ToList()
                    }).ToList()
                };

                response.IsSuccess = true;
                response.Data = routinesResponse;
                response.Message = "Lista de rutinas obtenida exitosamente.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<bool>> UpdateExercise(UpdateExerciseRequestDto updateExerciseRequestDto)
        {
            var response = new BaseResponse<bool>();
            string role = _jwtHandler.GetRoleFromToken();
            if (role != "gimnasio")
            {
                response.IsSuccess = false;
                response.Message = "No autorizado";
                return response;
            }
            var gymID = _jwtHandler.ExtractIdFromToken();

            try
            {
                var exercise = await _unitOfWork.ExerciseRepository.GetExerciseById(updateExerciseRequestDto.ExerciseId);

                if (exercise == null || exercise.IdGym != gymID)
                {
                    response.IsSuccess = false;
                    response.Message = "El ejercicio no existe o no pertenece a este gimnasio.";
                    return response;
                }

                exercise.ExerciseTitle = updateExerciseRequestDto.ExerciseTitle;
                exercise.ExerciseDescription = updateExerciseRequestDto.ExerciseDescription;
                exercise.Duration = updateExerciseRequestDto.Duration;
                exercise.VideoURL = updateExerciseRequestDto.VideoURL;
                exercise.ImageURL = updateExerciseRequestDto.ImageURL;
                exercise.IdMuscleGroup = updateExerciseRequestDto.IdMuscleGroup ?? null;
                exercise.UpdatedAt = DateTime.Now;
                exercise.IsActive = updateExerciseRequestDto.IsActive;

                var exerciseUpdated = await _unitOfWork.ExerciseRepository.UpdateExercise(exercise);
                if (!exerciseUpdated)
                {
                    response.IsSuccess = false;
                    response.Message = "No se pudo actualizar el ejercicio.";
                    return response;
                }

                response.IsSuccess = true;
                response.Message = "El ejercicio fue actualizado exitosamente.";
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}
