using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorkoutPrograms.Application.Features.WorkoutPrograms.CreateWorkoutProgram;
using WorkoutPrograms.Application.Features.WorkoutPrograms.DeleteWorkoutProgram;
using WorkoutPrograms.Application.Features.WorkoutPrograms.GetWorkoutProgramById;
using WorkoutPrograms.Application.Features.WorkoutPrograms.GetWorkoutProgramList;
using WorkoutPrograms.Application.Features.WorkoutPrograms.UpdateWorkoutProgram;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.AddWorkoutProgramSplit;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.DeleteWorkoutProgramSplit;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.GetWorkoutProgramSplits;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.UpdateWorkoutProgramSplit;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.AddExerciseToSplit;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.GetSplitExercises;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.RemoveSplitExercise;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.UpdateSplitExercise;

namespace FitnessTracking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class WorkoutProgramsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WorkoutProgramsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Request models (API layer)
        public sealed class CreateWorkoutProgramRequest
        {
            public string Name { get; init; } = default!;
            public DateTime StartDate { get; init; }
            public DateTime EndDate { get; init; }
        }

        public sealed class UpdateWorkoutProgramRequest
        {
            public string Name { get; init; } = default!;
            public DateTime StartDate { get; init; }
            public DateTime EndDate { get; init; }
        }


        // GET: api/workoutprograms
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            // TODO: replace with real MediatR query when you add it:
            var result = await _mediator.Send(new GetWorkoutProgramListQuery(), cancellationToken);
            return Ok(result);
        }

        // GET: api/workoutprograms/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            // TODO: replace with real MediatR query when you add it:
            var result = await _mediator.Send(new GetWorkoutProgramByIdQuery { Id = id }, cancellationToken);
            if (result is null) return NotFound();
            return Ok(result);
        }

        public sealed class WorkoutProgramExerciseDto
        {
            public Guid WorkoutProgramExerciseId { get; init; }
            public Guid ExerciseId { get; init; }
            public string ExerciseName { get; init; } = default!;
            public int Sets { get; init; }
            public int TargetReps { get; init; }
        }

        // bu endpoint programa eklenmiþ tüm exercises'i dönecek þuan böyle bir feature yok sonra yazarýz
        // GET: api/workoutprograms/{programId}/exercises
        //[HttpGet("{programId:guid}/exercises")]
        //public async Task<IActionResult> GetProgramExercises(
        //    Guid programId,
        //    CancellationToken cancellationToken)
        //{
        //    var result = await _mediator.Send(
        //        new GetSplitExercisesQuery { WorkoutProgramId = programId },
        //        cancellationToken);

        //    return Ok(result);
        //}

        // POST: api/workoutprograms
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateWorkoutProgramRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            // TODO: replace with real MediatR command when you add it:
            var id = await _mediator.Send(
                new CreateWorkoutProgramCommand { Name = request.Name, StartDate = request.StartDate, EndDate = request.EndDate },
                cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        // PUT: api/workoutprograms/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateWorkoutProgramRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            // TODO: replace with real MediatR command when you add it:
            var success = await _mediator.Send(
                new UpdateWorkoutProgramCommand { Id = id, Name = request.Name, StartDate = request.StartDate, EndDate = request.EndDate },
                cancellationToken);

            return NoContent();

        }

        // DELETE: api/workoutprograms/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            // TODO: replace with real MediatR command when you add it:
            var success = await _mediator.Send(new DeleteWorkoutProgramCommand { Id = id }, cancellationToken);
            return NoContent();
        }

        // ---------- Program Splits (child entity of WorkoutProgram) ----------

        public sealed class AddSplitRequest
        {
            public string Name { get; init; } = default!;
            public int Order { get; init; }
        }

        public sealed class UpdateSplitRequest
        {
            public string Name { get; init; } = default!;
            public int Order { get; init; }
        }

        // GET: api/workoutprograms/{programId}/splits
        [HttpGet("{programId:guid}/splits")]
        public async Task<IActionResult> GetSplits(
            Guid programId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetWorkoutProgramSplitsQuery { WorkoutProgramId = programId },
                cancellationToken);

            return Ok(result);
        }

        [HttpPost("{programId:guid}/splits")]
        public async Task<ActionResult<Guid>> AddSplit(
            Guid programId,
            [FromBody] AddSplitRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var command = new AddWorkoutProgramSplitCommand(programId, request.Name, request.Order);
            var splitId = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetSplits), new { programId }, splitId);
        }

        // PUT: api/workoutprograms/{programId}/splits/{splitId}
        [HttpPut("{programId:guid}/splits/{splitId:guid}")]
        public async Task<IActionResult> UpdateSplit(
            Guid programId,
            Guid splitId,
            [FromBody] UpdateSplitRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var success = await _mediator.Send(
                new UpdateWorkoutProgramSplitCommand
                {
                    WorkoutProgramId = programId,
                    SplitId = splitId,
                    Name = request.Name,
                    Order = request.Order
                },
                cancellationToken);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/workoutprograms/{programId}/splits/{splitId}
        [HttpDelete("{programId:guid}/splits/{splitId:guid}")]
        public async Task<IActionResult> DeleteSplit(
            Guid programId,
            Guid splitId,
            CancellationToken cancellationToken)
        {
            var success = await _mediator.Send(
                new DeleteWorkoutProgramSplitCommand
                {
                    WorkoutProgramId = programId,
                    SplitId = splitId
                },
                cancellationToken);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }



        // ---------- Program Exercises (child entity of WorkoutProgram) ----------

        public sealed class AddProgramExerciseRequest
        {
            public Guid ExerciseId { get; init; }
            public int Sets { get; init; }
            public int TargetReps { get; init; }
        }

        public sealed class UpdateProgramExerciseRequest
        {
            public int Sets { get; init; }
            public int TargetReps { get; init; }
        }

        // GET: api/workoutprograms/{programId}/exercises
        [HttpGet("{programId:guid}/splits/{splitId:guid}/exercises")]
        public async Task<IActionResult> GetSplitExercises(Guid programId, Guid splitId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetSplitExercisesQuery { WorkoutProgramId = programId, WorkoutSplitId = splitId },
                cancellationToken);

            return Ok(result);
        }

        // POST: api/workoutprograms/{programId}/splits/{splitId}/exercises
        [HttpPost("{programId:guid}/splits/{splitId:guid}/exercises")]
        public async Task<IActionResult> AddExerciseToSplit(
            Guid programId,
            Guid splitId,
            [FromBody] AddProgramExerciseRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var workoutProgramExerciseId = await _mediator.Send(
                new AddExerciseToSplitCommand
                {
                    WorkoutProgramId = programId,
                    WorkoutProgramSplitId = splitId,
                    ExerciseId = request.ExerciseId,
                    Sets = request.Sets,
                    TargetReps = request.TargetReps
                },
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = programId },
                workoutProgramExerciseId);
        }

        // PUT: api/workoutprograms/{programId}/splits/{splitId}/exercises/{workoutProgramExerciseId}
        [HttpPut("{programId:guid}/splits/{splitId:guid}/exercises/{workoutProgramExerciseId:guid}")]
        public async Task<IActionResult> UpdateExerciseInSplit(
            Guid programId,
            Guid splitId,
            Guid workoutProgramExerciseId,
            [FromBody] UpdateProgramExerciseRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var success = await _mediator.Send(
                new UpdateSplitExerciseCommand
                {
                    WorkoutProgramId = programId,
                    WorkoutProgramSplitId = splitId,
                    WorkoutProgramExerciseId = workoutProgramExerciseId,
                    Sets = request.Sets,
                    TargetReps = request.TargetReps
                },
                cancellationToken);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/workoutprograms/{programId}/splits/{splitId}/exercises/{workoutProgramExerciseId}
        [HttpDelete("{programId:guid}/splits/{splitId:guid}/exercises/{workoutProgramExerciseId:guid}")]
        public async Task<IActionResult> RemoveExerciseFromSplit(
            Guid programId,
            Guid splitId,
            Guid workoutProgramExerciseId,
            CancellationToken cancellationToken)
        {
            var success = await _mediator.Send(
                new RemoveSplitExerciseCommand
                {
                    WorkoutProgramId = programId,
                    WorkoutProgramSplitId = splitId,
                    WorkoutProgramExerciseId = workoutProgramExerciseId
                },
                cancellationToken);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}