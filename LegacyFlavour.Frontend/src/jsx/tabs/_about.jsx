import React from 'react'
import $IconPanel from '../components/_icon-panel';
import $Button from '../components/_button';
import $Paragraph from '../components/_paragraph';
import $Description from '../components/_description';

const $About = ({ react, locale }) => {
    const launchReddit = (url) => {
        engine.trigger("cities2modding_legacyflavour.launchUrl", "https://www.reddit.com/r/cities2modding");
    };
    const launchGitHub = (url) => {
        engine.trigger("cities2modding_legacyflavour.launchUrl", "https://github.com/Cities2Modding");
    };
    const launchDiscord = (url) => {
        engine.trigger("cities2modding_legacyflavour.launchUrl", "https://discord.gg/KGRNBbm5Fh");
    };

    function toParagraph(str) {
        return <p cohinline="cohinline" dangerouslySetInnerHTML={{ __html: str }} />;
    }

    const title = `${locale["LEGACY_FLAVOUR"]} v1.0.1`;
    return <div>
        <$IconPanel label={title} style={{ flex: 1 }}
            description={locale["DEVELOPED_BY"]}
            icon="Media/Editor/Object.svg" fitChild="true">
        </$IconPanel>
        <div style={{ display: 'flex', flexDirection: 'column', width: '100%' }}>
            <$Description style={{ paddingTop: '0' }}>
                {toParagraph(locale["ABOUT_1"])}
                {toParagraph(locale["ABOUT_2"])}
                {toParagraph(locale["ABOUT_3"])}
            </$Description>
        </div>
        <div style={{ display: 'flex', flexDirection: 'row', flex: 1 }}>
            <div style={{ flex: 1 }}>
                <$IconPanel label="GitHub"
                    description={locale["GITHUB_DESC"]}
                    icon="https://raw.githubusercontent.com/prplx/svg-logos/master/svg/github-icon.svg" fitChild="true">
                    <div style={{ display: 'flex', flexDirection: 'column', width: '100%', padding: '10rem' }}>
                        <$Button isBlack="true" onClick={launchGitHub}>https://github.com/Cities2Modding</$Button>
                    </div>
                </$IconPanel>
                <$IconPanel label="Reddit"
                    description={locale["REDDIT_DESC"]}
                    icon="https://www.svgrepo.com/download/14413/reddit.svg" fitChild="true">
                    <div style={{ display: 'flex', flexDirection: 'column', width: '100%', padding: '10rem' }}>
                        <$Button isBlack="true" onClick={launchReddit}>https://www.reddit.com/r/cities2modding</$Button>
                    </div>
                </$IconPanel>
            </div>
            <div style={{ flex: 1, marginLeft: '5rem' }}>
                <$IconPanel label="Discord"
                    description={locale["DISCORD_DESC"]}
                    icon="https://assets-global.website-files.com/6257adef93867e50d84d30e2/653714c1f22aef3b6921d63d_636e0a6ca814282eca7172c6_icon_clyde_white_RGB.svg" fitChild="true">
                    <div style={{ display: 'flex', flexDirection: 'column', width: '100%', padding: '10rem' }}>
                        <$Button isBlack="true" onClick={launchDiscord}>https://discord.gg/KGRNBbm5Fh</$Button>
                    </div>
                </$IconPanel>
            </div>
        </div>
    </div>
}

export default $About