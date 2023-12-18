/** @type {import('vite').UserConfig} */
export default {
    build: {
        outDir: "./public"
    },
    server: {
        watch: {
            ignored: [
                "**/*.fs" // don't watch f# files
            ]
        }
    }
}