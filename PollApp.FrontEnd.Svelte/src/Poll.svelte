<script>
	import { createEventDispatcher } from 'svelte';
	const dispatch = createEventDispatcher();
	let poll = {};
	let displayMode = 'loading';
	export function loadPoll(pollId)
	{
		displayMode = 'loading';
		if (pollId) {
            fetch('https://poll-app.azurewebsites.net/api/poll/' + pollId, { method: 'GET' })
            .then(function(response) {
                if (!response.ok) {
                    throw new Error ('HTTP status code' + response.status);
                }                
                return response.json();
            })
            .then(function (json) {
                poll = json;
                displayMode = 'loaded';
            })
            .catch (function (err) {
                displayMode = 'loadError';
            });
		}
		else {
			displayMode = 'loadError';
		}
	}
	function answerPoll(answerId)
	{
		if (!answerId || !poll.id) {
			return;
		}
		fetch(`https://poll-app.azurewebsites.net/api/poll/${poll.id}/answer/${answerId}`, { method: 'POST' })
		.then(function(response) {
			if (!response.ok) {
                    throw new Error ('HTTP status code' + response.status);
			}
			dispatch('pollResponseCompleted');
		})
		.catch(function(err) {
			alert('Error while submitting answer');
		});
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
					<button type="button" class="btn btn-primary float-right" on:click={answerPoll(answer.id)}>Vote</button>
					</li>
				{/each}
			</ul>
		</div>
	{:else if displayMode == 'loadError'}
		<h1>Unable to load poll</h1>
	{:else}
		<h1>Loading...</h1>
	{/if}
</main>

<style>
</style>