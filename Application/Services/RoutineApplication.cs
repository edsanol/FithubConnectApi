using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

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
                    UpdatedAt = DateTime.Now
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
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
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
    }
}
