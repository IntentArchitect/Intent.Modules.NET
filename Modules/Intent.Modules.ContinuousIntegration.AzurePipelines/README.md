# Intent.ContinuousIntegration.AzurePipelines

This module generates a basic `azure-pipeline.yml` for [Azure Pipelines](https://learn.microsoft.com/azure/devops/pipelines/?view=azure-devops) file which will:

- Build all `.csproj` files in the repository.
- Run unit tests for any `.csproj` whose name is suffixed with `Tests`.
- Run the [Intent Architect Software Factory CLI](https://docs.intentarchitect.com/articles/cli-tools/software-factory-cli/software-factory-cli.html) tool to detect possible outstanding changes.

## Creating a pipeline

This readme assumes you have some familiarity with Azure DevOps Pipelines, for more information on creating a Pipeline, refer to Microsoft's [_Create your first pipeline_ article](https://learn.microsoft.com/azure/devops/pipelines/create-first-pipeline?view=azure-devops&tabs=net%2Ctfs-2018-2%2Cbrowser).

- Click on the `Pipelines` option in the pane on the left.
- Click the `Create Pipeline` button.

![Pipelines view (empty)](content/images/pipelines-view-empty.jpeg)

- Follow the instructions on how to choose a source code repository.
- After selecting your source code repository, it should automatically find the `azure-pipelines.yml` file which you can `Save and run`:

![New pipeline review](content/images/new-pipeline-review.jpeg)

After the first run is complete, the pipeline should look somewhat like this:

![After first pipeline run](content/images/after-first-run.jpeg)

If you have a warning saying `Project file(s) matching the specified pattern were not found.`, this is because it didn't locate any "Test" projects to run. Once you have added unit test projects, the warning should disappear. You will also need to ensure that your unit test project names are suffixed with `Tests`.

If you have a warning starting with `Intent Architect Credentials not configured…`, this is because you haven't yet [configured Intent Architect Credentials](#configuring-intent-architect-credentials) for the pipeline.

## Configuring Intent Architect Credentials

Because valid Intent Architect account credentials are required to run the the [Intent Architect Software Factory CLI](https://docs.intentarchitect.com/articles/cli-tools/software-factory-cli/software-factory-cli.html), some quick additional configuration will be required to make them available to pipeline.

In the left navigation pane, go to `Library`.

![Azure Pipelines Library view](content/images/library-view.jpeg)

- Press the `+ Variable Group` button.
- Use `Intent Architect Credentials` for the `Variable Group Name`.
- Under variables, press the `+ Add` button.
- For the `Name` enter `intent-architect-user` and for the `Value` enter the name of a valid Intent Architect account.
- Under variables, press the `+ Add` button again.
- For the `Name` enter `intent-architect-password`, for the `Value` enter the password for the Intent Architect account and finally press the "Padlock" icon for this row which will make the variable [secret](https://learn.microsoft.com/azure/devops/pipelines/process/set-secret-variables?view=azure-devops&tabs=yaml%2Cpowershell).

![New Variable Group](content/images/new-variable-group.jpeg)

Return to the Pipelines view by using the `Pipelines` option in the navigation pane on the left.

![Pipelines runs view](content/images/pipelines-view-runs.jpeg)

- Press the `Edit` button at the top right of the screen.
- Uncomment the `# - group: 'Intent Architect Credentials'` line by removing the leading `#` so that it becomes to `- group: 'Intent Architect Credentials'`.

![Edited azure-pipeline.yml file](content/images/pipeline-edit.jpeg)

Press the `Save` button at the top right of the screen.

![Save dialogue](content/images/pipeline-edit-save.jpeg)

On the dialogue update any details as desired and press the `Save` button at the bottom right.

![azure-pipeline.yml file saved](content/images/pipeline-edit-run.jpeg)

- You can now press the `Run` button at the top right of the screen.
- On the `Run Pipeline` dialogue (although not essential, you can update any details as desired and) press the `Run` button at the bottom right.

![Run pipeline dialogue](content/images/pipeline-edit-run-parameters.jpeg)

Once the pipeline has started, you will see that it shows a yellow box saying `This pipeline needs permission to access a resource before this run can continue`.

![Pipeline needs permission](content/images/pipeline-run-needs-permission.jpeg)

Press the `View` button in this yellow box.

![Waiting for review dialogue](content/images/pipeline-run-needs-permission-view.jpeg)

In the `Waiting for review` dialogue press the `Permit` button.

![image](content/images/pipeline-run-needs-permission-permit.jpeg)

Press `Permit` on the additional dialogue.

The pipeline should now continue and from this time onwards it will run the Intent Architect Software Factory CLI without further permission granting required.
