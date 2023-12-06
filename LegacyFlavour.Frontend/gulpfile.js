const gulp = require('gulp');
const esbuild = require('esbuild');
const fs = require('fs-extra'); // fs-extra is an extended version of Node's fs module

gulp.task('build-jsx', function (done) {
    esbuild.build({
        entryPoints: ['src/jsx/ui.jsx'],
        outdir: '../LegacyFlavour/Resources',
        bundle: true,
        platform: 'browser',
        loader: {
            '.js': 'jsx',
            '.jsx': 'jsx'
        }
        // Add other esbuild options as needed
    }).then(() => {
        // After successful build, copy the file to the target directory
        fs.copySync(
            '../LegacyFlavour/Resources/ui.js',
            'G:/SteamLibrary/steamapps/common/Cities Skylines II/Cities2_Data/StreamingAssets/~UI~/HookUI/Extensions/panel.cities2modding.legacyflavour.js'
        );
        done();
    }).catch((error) => {
        console.error(error);
        done(new Error('Build failed'));
    });
});

gulp.task('default', gulp.series('build-jsx'));
