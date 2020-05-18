import App from './App.svelte';

const app = new App({
	target: document.body,
	props: {
		pollApiHost: poll_api_host
	}
});
export default app;
