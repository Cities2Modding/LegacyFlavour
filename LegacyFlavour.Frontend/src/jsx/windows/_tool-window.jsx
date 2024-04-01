import React from "react";

const ToolWindow = ({ model, update, trigger, _L }) => {
    const react = window.$_gooee.react;
    const { Button, Icon, Modal } = window.$_gooee.framework;
    
    const onSelectTool = (tool) => {
        const isZoneTool = tool.id === "Zone Tool";

        if (!isZoneTool) {
            update("IsToolVisible", false);
        }
        else {
            update("IsToolVisible", true);
        }
    };

    react.useEffect(() => {
        const selectToolHandle = engine.on("tool.activeTool.update", onSelectTool);

        return () => {
            selectToolHandle.clear();
        };
    }, [model.IsVisible])

    const closeModal = () => {
        trigger("OnToggleVisible");
        engine.trigger("audio.playSound", "close-panel", 1);
    };

    const isVisibleClass = "tool-layout";

    return model.IsToolVisible ? <div className={isVisibleClass}>
        <div className="col">
        </div>
        <div className="col">            
        </div>
        <div className="col">
            <div className="bg-panel text-light rounded-sm p-4">
                Zone settings
            </div>
        </div>
    </div> : null;
};

export default ToolWindow;