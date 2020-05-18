<script>
	import Poll from './Poll.svelte';
	import PollResult from './PollResult.svelte';
	import { onMount } from 'svelte';

	let poll;
	let showResults = false;
	let pollId;
	export let pollApiHost;
	let pollIdToLoad;
	$: {
		if (pollId && poll) {
			poll.loadPoll(pollId);
		}
	}
	onMount(() => {
		var params = new URLSearchParams(window.location.search);
		pollId = params.get('poll');
	});
	function pollResponseCompleted() {
		showResults = true;
	}
	function loadPoll() {
		window.location = "/?poll=" + pollIdToLoad.toLowerCase();
	}
</script>

<main>
	<nav class="navbar navbar-dark bg-primary">
		<span class="navbar-brand">Poll</span>
	</nav>
	<div class="container-sm">
		<div class="row">
			<div class="col">&nbsp;</div>
		</div>
		{#if pollId }
		<div class="row justify-content-center">
			<div class="col-lg-8 col-12">
				{#if showResults == false}
				<Poll pollApiHost="{pollApiHost}" bind:this="{poll}" on:pollResponseCompleted="{pollResponseCompleted}"></Poll>
				{:else}
				<PollResult pollApiHost="{pollApiHost}" pollId="{pollId}"></PollResult>
				{/if}
			</div>
		</div>
		{:else}
		<div class="row justify-content-center">
			<div class="col-lg-8 col-12">
				<form>
					<div class="form-group">
						<label for="poll">Poll Id</label>
						<input type="text" class="form-control" id="poll" placeholder="Identifier of the poll to view" bind:value="{pollIdToLoad}" />
					</div>
					<button type="button" class="btn btn-primary" on:click="{loadPoll}">View Poll</button>
				</form>
			</div>
		</div>
		{/if}
	</div>
</main>

<style>
</style>