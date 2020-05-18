Run Svelte Locally
------------------

This will run Svelte with a server that provides auto-compilation and hot-reload. It assumes you are running the PollApp.Web using dotnet at the following address https://localhost:50001/.

Once you are happy with your changes, you need to compile the Svelte output, since the normal execution of PollApp.Web uses the build output of Svelte and does NOT require npm to execute, see Build Svelte Output below.

1. First time only, install packages
```
npm install
```    

2. Run Svelte in Watch mode
```
npm run dev
```

Build Svelte Output
-------------------

The PollApp.Web serves static content only from it's wwwroot/ folder. It is serving pre-compiled output from Svelte. To update that content, simply run the build.ps1 powershell command.

```
build.ps1
```

Checkin the changes to the Git repository.