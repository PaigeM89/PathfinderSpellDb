import { defineConfig, loadEnv } from 'vite'

/** @type {import('vite').UserConfig} */
export default defineConfig(({command, mode}) => {
    const environment = loadEnv(mode, process.cwd(), 'VERSION');
    
    console.log('ENVIRONMENT', environment)

    return {
        build: {
            outDir: "./public"
        },
        server: {
            watch: {
                ignored: [
                    "**/*.fs" // don't watch f# files
                ]
            }
        },
        define: {
            ENV: environment,
        }
    }
})