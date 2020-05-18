<script>
	import Poll from './Poll.svelte';
	import PollResult from './PollResult.svelte';
	import { onMount } from 'svelte';

	let poll;
	let showResults = false;
	let pollId;
	onMount(() => {
		var params = new URLSearchParams(window.location.search);
		pollId = params.get('poll');
		if (poll)
		{
			poll.loadPoll(pollId);
		}
	});
	function pollResponseCompleted() {
		showResults = true;
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
		<div class="row justify-content-center">
			<div class="col-lg-8 col-12">
				{#if showResults == false}
				<Poll bind:this="{poll}" on:pollResponseCompleted="{pollResponseCompleted}"></Poll>
				{:else}
				<PollResult pollId="{pollId}"></PollResult>
				{/if}
			</div>
		</div>
		<div class="row justify-content-center">
			<div class="col-lg-8 col-12">
			</div>
		</div>
	</div>
</main>

<style>
</style>