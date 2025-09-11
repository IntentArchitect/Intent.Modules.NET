# Intent.Aws.Lambda.Functions

Serverless AWS Lambda Functions modeling and generation for the Services designer.

This module lets you model operations (e.g. service operations / commands / queries) that will be exposed as AWS Lambda functions and generates the supporting project assets to build, test and deploy them using idiomatic .NET. It focuses on what the module can scaffold for you – not every AWS Lambda feature.

## What It Generates

When applied to an AWS Lambda API host project, the module produces:

* Lambda function classes leveraging `Amazon.Lambda.Annotations` attributes. Each class will have their own cohesive set of methods representing endpoints.
* A startup / bootstrap class wiring logging (CloudWatch via `Amazon.Lambda.Logging.AspNetCore`) and dependency injection.
* A `serverless.template` (CloudFormation) produced at build time through source generators (driven by the annotations framework) – you don't hand author it here.
* Optional response DTO mapping helpers (Json response template) when responses are modeled.
* `aws-lambda-tools-defaults.json` (via the AwsLambdaToolsDefaults template) with sensible defaults for packaging and deployment.
* A `samconfig.toml` (if the SAM config template is included) for SAM CLI convenience.
* A README (per generated application) giving quick start guidance (see `AwsLambdaReadMeTemplate`).

## Modeling

Model your functions in the Services designer using service operations / commands / queries. Each exposed operation becomes a Lambda function class with the appropriate annotation. Typical HTTP/APIGateway style triggers are supported through the `Amazon.Lambda.Annotations` framework (e.g. mapping to routes / verbs) according to what the template emits.

## Configuration

Generated projects use standard .NET configuration (appsettings + environment variables). Environment variables override JSON values, enabling environment-specific configuration without code changes.

## Local Development

Two primary local options are supported in the generated guidance:

1. Mock Lambda Test Tool: Launchable via a `launchSettings.json` profile (`dotnet-lambda-test-tool-8.0.exe`) to invoke functions locally with sample events.
2. SAM CLI: You can build and run the project locally using:
	* `sam build --template serverless.template`
	* `sam local start-api`

The SAM usage relies on the CloudFormation template produced at build time by the annotations/source generator pipeline.

## Deployment

Deployment is performed with the `Amazon.Lambda.Tools` .NET global tool:

1. Prerequisites:
	* AWS account (create one at https://aws.amazon.com/console/ if needed).
	* IAM user/role with permissions to deploy CloudFormation stacks and Lambda functions (credentials stored in `~/.aws/credentials`).
2. Install tool (if missing): `dotnet tool install --global Amazon.Lambda.Tools`.
3. Deploy: `dotnet lambda deploy-serverless` from the project directory. This packages the code and deploys the generated CloudFormation stack.

## Resources

* Amazon.Lambda.Annotations: https://github.com/aws/aws-lambda-dotnet/tree/master/Libraries/src/Amazon.Lambda.Annotations
* AWS .NET CLI Extensions (deployment tooling): https://github.com/aws/aws-extensions-for-dotnet-cli
* AWS Lambda for .NET repo: https://github.com/aws/aws-lambda-dotnet

---

For release history see `release-notes.md`.
