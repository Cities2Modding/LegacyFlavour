import React from "react";

const $IconPanel = ({ label, description, icon, fitChild, children, style }) => {
    const conditionalInsert = !fitChild ?
        <div className="d-flex justify-content-center" style={{ width: "75rem" }}>
            {children}
        </div> : "";

    const conditionalInsert2 = fitChild ?
        <div className="d-flex justify-content-center w-100">
            {children}
        </div> : "";

    return <div className="bg-section-dark rounded-sm mb-4" style={{...style }}>
        <div className="d-flex flex-row">
            <div className="d-flex justify-content-center" style={{ width: "75rem" }}>
                <img className="m-4 align-self-center" style={{ maxWidth: "55rem", maxHeight: "55rem" }} src={icon} />
            </div>
            <div className="flex-1 mt-4 mb-4">
                <h4 className="text-primary mb-0">{label}</h4>
                <div>
                    <p cohinline="cohinline" className="text-muted pr-4">
                        {description}
                    </p>
                </div>
            </div>
            {conditionalInsert}
        </div>
        {conditionalInsert2}
    </div>
}

export default $IconPanel