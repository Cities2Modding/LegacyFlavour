import React from 'react'
import $IconPanel from '../components/_icon-panel';
import $Button from '../components/_button';

const $About = ({ locale }) => {
    const { Icon, Grid, Button } = window.$_gooee.framework;
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
        return <p className="mb-4 fs-l" cohinline="cohinline" dangerouslySetInnerHTML={{ __html: str }} />;
    }

    const title = `${locale["LEGACY_FLAVOUR"]} v1.1.1`;
    return <div>
        <div className="bg-section-dark rounded-sm p-4 d-flex flex-row align-items-center">
            <Icon icon="Media/Editor/Object.svg" size="xl" />
            <div className="flex-1 ml-2">
                <h1 className="text-primary">{title}</h1>
                <div className="text-muted fs-xl">{locale["DEVELOPED_BY"]}</div>
            </div>
        </div>
        <div className="d-block p-4">
            {toParagraph(locale["ABOUT_1"])}
            {toParagraph(locale["ABOUT_2"])}
            {toParagraph(locale["ABOUT_3"])}
        </div>
        <div>
            <Grid auto>
                <div>
                    <div className="bg-section-dark rounded-sm d-flex flex-row align-items-start p-4 flex-1">
                        <Icon icon="brand-github" size="xl" fa />
                        <div className="flex-1 w-x ml-4">
                            <div className="text-primary">
                                GitHub
                            </div>
                            <p className="text-muted mb-4" cohinline="cohinline">
                                {locale["GITHUB_DESC"]}
                            </p>
                        </div>
                    </div>
                </div>
                <div>
                    <div className="bg-section-dark rounded-sm d-flex flex-row align-items-start p-4 flex-1">
                        <Icon icon="brand-reddit" size="xl" fa />
                        <div className="flex-1 w-x ml-4">
                            <div className="text-primary">
                                Reddit
                            </div>
                            <p className="text-muted mb-4" cohinline="cohinline">
                                {locale["REDDIT_DESC"]}
                            </p>
                        </div>
                    </div>
                </div>
                <div>
                    <div className="bg-section-dark rounded-sm d-flex flex-row align-items-start p-4 flex-1">
                        <Icon icon="brand-discord" size="xl" fa />
                        <div className="flex-1 w-x ml-4">
                            <div className="text-primary">
                                Discord
                            </div>
                            <p className="text-muted mb-4" cohinline="cohinline">
                                {locale["DISCORD_DESC"]}
                            </p>
                         </div>
                    </div>
                </div>
            </Grid>
            <Grid className="mt-4" auto>
                <div>
                    <Button className="pl-4 pr-2 text-center" isBlock color="primary" style="trans" size="sm" onClick={launchGitHub}>https://github.com/Cities2Modding</Button>
                </div>
                <div>
                    <Button className="pl-2 pr-2 text-center" isBlock color="primary" style="trans" size="sm" onClick={launchReddit}>https://www.reddit.com/r/cities2modding</Button>
                </div>
                <div>
                    <Button className="pl-2 pr-4 text-center" isBlock color="primary" style="trans" size="sm" onClick={launchDiscord}>https://discord.gg/KGRNBbm5Fh</Button>
                </div>
            </Grid>
        </div>
    </div>
}

export default $About