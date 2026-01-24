using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class WorkoutSessionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WorkoutSessionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Bu sýnýftaki request modelleri ve MediatR çaðrýlarý,
        // WorkoutSession aggregate'ini tanýmladýðýnda doldurulmak üzere býrakýldý.

        // Örnek request modelleri (aggregate tasarýmýna göre güncelle):
        public sealed class CreateWorkoutSessionRequest
        {
            public Guid WorkoutProgramId { get; init; }
            public DateTime Date { get; init; }
        }

        public sealed class UpdateWorkoutSessionRequest
        {
            public DateTime Date { get; init; }
        }

        // GET: api/workoutsessions
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            // TODO: _mediator.Send(new GetAllWorkoutSessionsQuery(), cancellationToken);
            return Ok();
        }

        // GET: api/workoutsessions/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(_mediator.Send(new GetWorkoutSessionByIdQuery { Id = id }, cancellationToken));
        }

        // POST: api/workoutsessions
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateWorkoutSessionRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            // TODO:
            var id = await _mediator.Send(
                 new CreateWorkoutSessionCommand { WorkoutProgramId = request.WorkoutProgramId, Date = request.Date },
                 cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        // PUT: api/workoutsessions/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateWorkoutSessionRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            // TODO:
            var success = await _mediator.Send(
                new UpdateWorkoutSessionCommand { Id = id, Date = request.Date, },
                cancellationToken);

            return NoContent();
        }

        // DELETE: api/workoutsessions/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            // TODO:
            var success = await _mediator.Send(new DeleteWorkoutSessionCommand { Id = id }, cancellationToken);

            return NoContent();
        }


        // ---------- WorkoutExercise (session içindeki setler) ----------

        public sealed class AddWorkoutExerciseRequest
        {
            public Guid ExerciseId { get; init; }
            public int SetNumber { get; init; }
            public decimal Weight { get; init; }
            public int Reps { get; init; }
        }

        public sealed class UpdateWorkoutExerciseRequest
        {
            public int SetNumber { get; init; }
            public decimal Weight { get; init; }
            public int Reps { get; init; }
        }

        // POST: api/workoutsessions/{sessionId}/exercises
        // Sadece programa kayýtlý ExerciseId'ler handler içinde kabul edilecek.
        [HttpPost("{sessionId:guid}/exercises")]
        public async Task<IActionResult> AddWorkoutExercise(
            Guid sessionId,
            [FromBody] AddWorkoutExerciseRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var workoutExerciseId = await _mediator.Send(
                new AddWorkoutExerciseToSessionCommand
                {
                    WorkoutSessionId = sessionId,
                    ExerciseId = request.ExerciseId,
                    SetNumber = request.SetNumber,
                    Weight = request.Weight,
                    Reps = request.Reps
                },
                cancellationToken);

            // Programda olmayan exercise için handler'dan InvalidOperationException
            // döndürüp burada 400/409'a map edebilirsin (exception middleware ile).
            return CreatedAtAction(
                nameof(GetById),
                new { id = sessionId },
                workoutExerciseId);
        }

        // PUT: api/workoutsessions/{sessionId}/exercises/{workoutExerciseId}
        [HttpPut("{sessionId:guid}/exercises/{workoutExerciseId:guid}")]
        public async Task<IActionResult> UpdateWorkoutExercise(
            Guid sessionId,
            Guid workoutExerciseId,
            [FromBody] UpdateWorkoutExerciseRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            await _mediator.Send(
               new UpdateWorkoutExerciseInSessionCommand
               {
                   WorkoutSessionId = sessionId,
                   WorkoutExerciseId = workoutExerciseId,
                   SetNumber = request.SetNumber,
                   Weight = request.Weight,
                   Reps = request.Reps
               },
               cancellationToken);

            return NoContent();
        }

        // DELETE: api/workoutsessions/{sessionId}/exercises/{workoutExerciseId}
        [HttpDelete("{sessionId:guid}/exercises/{workoutExerciseId:guid}")]
        public async Task<IActionResult> RemoveWorkoutExercise(
            Guid sessionId,
            Guid workoutExerciseId,
            CancellationToken cancellationToken)
        {
            var success = await _mediator.Send(
                new RemoveWorkoutExerciseFromSessionCommand
                {
                    WorkoutSessionId = sessionId,
                    WorkoutExerciseId = workoutExerciseId
                },
                cancellationToken);

            return NoContent();
        }
    }
}