//using MediatR;
//using Microsoft.AspNetCore.Mvc;
//using WorkoutSessions.Application.Feature.WorkoutSessions.ActivateWorkoutSession;
//using WorkoutSessions.Application.Feature.WorkoutSessions.CreateWorkoutSession;
//using WorkoutSessions.Application.Feature.WorkoutSessions.DeleteWorkoutSession;
//using WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessionById;
//using WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessions;
//using WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.ActivateSessionExercise;
//using WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.AddExerciseToSession;
//using WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.GetExercisesBySession;
//using WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.RemoveExerciseFromSession;
//using WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.UpdateExerciseInSession;
//using WorkoutSessions.Application.Feature.WorkoutSessions.UpdateWorkoutSession;

//namespace FitnessTracking.Api.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public sealed class WorkoutSessionsController : ControllerBase
//    {
//        private readonly IMediator _mediator;

//        public WorkoutSessionsController(IMediator mediator)
//        {
//            _mediator = mediator;
//        }

//        // Bu sýnýftaki request modelleri ve MediatR çaðrýlarý,
//        // WorkoutSession aggregate'ini tanýmladýðýnda doldurulmak üzere býrakýldý.

//        // Örnek request modelleri (aggregate tasarýmýna göre güncelle):
//        public sealed class CreateWorkoutSessionRequest
//        {
//            public Guid WorkoutProgramId { get; init; }
//            public DateTime Date { get; init; }
//        }

//        public sealed class UpdateWorkoutSessionRequest
//        {
//            public DateTime Date { get; init; }
//        }

//        // GET: api/workoutsessions
//        [HttpGet]
//        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
//        {
//            var result = await _mediator.Send(new GetWorkoutSessionsQuery(), cancellationToken);
//            return Ok(result);
//        }

//        // GET: api/workoutsessions/{id}
//        [HttpGet("{id:guid}")]
//        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
//        {
//            return Ok(await _mediator.Send(new GetWorkoutSessionByIdQuery { Id = id }, cancellationToken));
//        }

//        // POST: api/workoutsessions
//        [HttpPost]
//        public async Task<IActionResult> Create(
//            [FromBody] CreateWorkoutSessionRequest request,
//            CancellationToken cancellationToken)
//        {
//            if (!ModelState.IsValid)
//            {
//                return ValidationProblem(ModelState);
//            }

//            // TODO:
//            var id = await _mediator.Send(
//                 new CreateWorkoutSessionCommand { WorkoutProgramId = request.WorkoutProgramId, Date = request.Date },
//                 cancellationToken);

//            return CreatedAtAction(nameof(GetById), new { id }, id);
//        }

//        // PUT: api/workoutsessions/{id}
//        [HttpPut("{id:guid}")]
//        public async Task<IActionResult> Update(
//            Guid id,
//            [FromBody] UpdateWorkoutSessionRequest request,
//            CancellationToken cancellationToken)
//        {
//            if (!ModelState.IsValid)
//            {
//                return ValidationProblem(ModelState);
//            }

//            // TODO:
//            var success = await _mediator.Send(
//                new UpdateWorkoutSessionCommand { Id = id, Date = request.Date, },
//                cancellationToken);

//            return NoContent();
//        }

//        // DELETE: api/workoutsessions/{id}
//        [HttpDelete("{id:guid}")]
//        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
//        {
//            // TODO:
//            var success = await _mediator.Send(new DeleteWorkoutSessionCommand { Id = id }, cancellationToken);

//            return NoContent();
//        }

//        // PUT: api/workoutsessions/{workoutSessionId}/activate
//        [HttpPut("{workoutSessionId:guid}/activate")]
//        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        public async Task<IActionResult> ActivateWorkoutSession(Guid workoutSessionId, CancellationToken cancellationToken)
//        {
//            var resultId = await _mediator.Send(new ActivateWorkoutSessionCommand(workoutSessionId), cancellationToken);
//            return Ok(resultId);
//        }


//        // ---------- SessionExercise (session içindeki setler) ----------

//        public sealed class AddSessionExerciseRequest
//        {
//            public Guid ExerciseId { get; init; }
//            public int SetNumber { get; init; }
//            public decimal Weight { get; init; }
//            public int Reps { get; init; }
//        }

//        public sealed class UpdateSessionExerciseRequest
//        {
//            public int SetNumber { get; init; }
//            public decimal Weight { get; init; }
//            public int Reps { get; init; }
//        }

//        // POST: api/workoutsessions/{sessionId}/exercises
//        // Sadece programa kayýtlý ExerciseId'ler handler içinde kabul edilecek.
//        [HttpPost("{sessionId:guid}/exercises")]
//        public async Task<IActionResult> AddSessionExercise(
//            Guid sessionId,
//            [FromBody] AddSessionExerciseRequest request,
//            CancellationToken cancellationToken)
//        {
//            if (!ModelState.IsValid)
//            {
//                return ValidationProblem(ModelState);
//            }

//            var sessionExerciseId = await _mediator.Send(
//                new AddExerciseToSessionCommand
//                {
//                    WorkoutSessionId = sessionId,
//                    ExerciseId = request.ExerciseId,
//                    SetNumber = request.SetNumber,
//                    Weight = request.Weight,
//                    Reps = request.Reps
//                },
//                cancellationToken);

//            // Programda olmayan exercise için handler'dan InvalidOperationException
//            // döndürüp burada 400/409'a map edebilirsin (exception middleware ile).
//            return CreatedAtAction(
//                nameof(GetById),
//                new { id = sessionId },
//                sessionExerciseId);
//        }

//        // PUT: api/workoutsessions/{sessionId}/exercises/{sessionExerciseId}
//        [HttpPut("{sessionId:guid}/exercises/{sessionExerciseId:guid}")]
//        public async Task<IActionResult> UpdateSessionExercise(
//            Guid sessionId,
//            Guid sessionExerciseId,
//            [FromBody] UpdateSessionExerciseRequest request,
//            CancellationToken cancellationToken)
//        {
//            if (!ModelState.IsValid)
//            {
//                return ValidationProblem(ModelState);
//            }

//            await _mediator.Send(
//               new UpdateExerciseInSessionCommand
//               {
//                   WorkoutSessionId = sessionId,
//                   SessionExerciseId = sessionExerciseId, // deðiþecek
//                   SetNumber = request.SetNumber,
//                   Weight = request.Weight,
//                   Reps = request.Reps
//               },
//               cancellationToken);

//            return NoContent();
//        }

//        // GET: api/workoutsessions/{sessionId}/exercises/{sessionExerciseId}
//        [HttpGet("{sessionId:guid}/exercises/")]
//        public async Task<IActionResult> GetSessionExercises(
//            Guid sessionId,
//            CancellationToken cancellationToken)
//        {
//            var result = await _mediator.Send(
//                new GetExercisesBySessionQuery { WorkoutSessionId = sessionId },
//                cancellationToken);

//            return Ok(result);
//        }

//        // DELETE: api/workoutsessions/{sessionId}/exercises/{sessionExerciseId}
//        [HttpDelete("{sessionId:guid}/exercises/{sessionExerciseId:guid}")]
//        public async Task<IActionResult> RemoveSessionExercise(
//            Guid sessionId,
//            Guid sessionExerciseId,
//            CancellationToken cancellationToken)
//        {
//            var success = await _mediator.Send(
//                new RemoveExerciseFromSessionCommand
//                {
//                    WorkoutSessionId = sessionId,
//                    SessionExerciseId = sessionExerciseId
//                },
//                cancellationToken);

//            return NoContent();
//        }

//        // PUT: api/workoutsessions/{workoutSessionId}/exercises/{sessionExerciseId}/activate
//        [HttpPut("{workoutSessionId:guid}/exercises/{sessionExerciseId:guid}/activate")]
//        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        public async Task<IActionResult> ActivateSessionExercise(
//            Guid workoutSessionId,
//            Guid sessionExerciseId,
//            CancellationToken cancellationToken)
//        {
//            var resultId = await _mediator.Send(
//                new ActivateSessionExerciseCommand(workoutSessionId, sessionExerciseId),
//                cancellationToken);

//            return Ok(resultId);
//        }
//    }
//}