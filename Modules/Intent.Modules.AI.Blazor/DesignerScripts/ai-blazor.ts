const navigationSourceEndSpecializationId = "97a3de8a-c9bf-4cf2-bc0a-b8692b02211b";
const navigationTargetEndSpecializationId = "2b191288-ecae-4743-b069-cbdd927ef349";
const showDialogTargetEndSpecializationId = "c44a7969-abfa-4073-ab2c-d2d0f1f6bd2f";

async function createComponentImplementAICodingTask() {
    let filePaths = element.getAssociatedFiles().map(x => x.absolutePath);

    let intention = '';
    element.getAssociations().filter(a => (a.specializationId == navigationSourceEndSpecializationId ||
        a.specializationId == navigationTargetEndSpecializationId) && a.typeReference.isNavigable).forEach(n => {
            intention += `- This pages navigates to the ${n.getName()} component${"\n"}`;
        });

    element.getChildren("e030c97a-e066-40a7-8188-808c275df3cb").forEach(o => {
        o.getAssociations().filter(a => a.specializationId == showDialogTargetEndSpecializationId).forEach(a => {
            intention += `- The ${o.getName()} operation opens a dialog to show the ${a.typeReference.getType().getName()} component${"\n"}`;
        });
    });

    createAICodingTask({
        title: `Implement Blazor Component: ${element.getName()}`,
        instructions: `Implement "${element.getName()}" using the appropriate skill(s).`,
        context: `
            ## User has modeled the following intentions:
            ${intention}`,
        filesToInclude: filePaths
    });
}