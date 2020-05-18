<script>
	let poll = {
	};
	let displayMode = 'loading';
	export let pollId = "";
	export let pollApiHost = "";
	var refreshInterval;
	function updatePollAndPercentages(json)
	{
		let totalResponseCount = json.possibleAnswers.reduce(function (sum, answer) {
			return sum + answer.responseCount;
		}, 0);
		json.possibleAnswers.forEach(function (answer) {
			answer.percentage = 0;
			if (totalResponseCount > 0) {
				answer.percentage = (answer.responseCount / totalResponseCount).toFixed(2) * 100;
			}
		});
		poll = json;
	}
	function refreshPoll() {
		fetch(`${pollApiHost}/api/poll/${pollId}`, { method: 'GET' })
		.then(function(response) {
			if (!response.ok) {
				throw new Error ('HTTP status code' + response.status);
			}                
			return response.json();
		}).then(function (json) {
			updatePollAndPercentages(json);
		}).catch(function (err) {
			clearInterval(refreshInterval);
		});
	}
	function startPollRefresh()
	{
		refreshInterval = setInterval(refreshPoll, 500);
	}
	if (pollId) {
		fetch(`${pollApiHost}/api/poll/${pollId}`, { method: 'GET' })
		.then(function(response) {
			if (!response.ok) {
				throw new Error ('HTTP status code' + response.status);
			}                
			return response.json();
		})
		.then(function (json) {
			updatePollAndPercentages(json);
			startPollRefresh();			
			displayMode = 'loaded';
		})
		.catch (function (err) {
			displayMode = 'loadError';
		});
	}
	else {
		displayMode = 'loadError';
	}
</script>

<main>
	{#if displayMode == 'loaded'}
		<div class="card">
		   <div class="card-header">{poll.question}</div>		   
			<ul class="list-group list-group-flush">
				{#each poll.possibleAnswers as answer} 
					<li class="list-group-item">
						<div class="pb-2">{answer.text}</div>
						<div class="progress">
							<div class="progress-bar" role="progressbar" style="width: {answer.percentage}%" aria-valuenow="{answer.percentage}" aria-valuemin="0" aria-valuemax="100">{answer.responseCount}</div>
						</div>
					</li>
				{/each}
			</ul>
		</div>
	{:else if displayMode == 'loadError'}
		<h1>Unable to load poll results</h1>
	{:else}
		<h1>Loading...</h1>
	{/if}
</main>

<style>
</style>