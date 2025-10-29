### Overview

This is a .NET AWS Lambda project that leverages the **Amazon.Lambda.Annotations** framework to streamline the creation of serverless functions. This approach allows for idiomatic C# coding patterns, with boilerplate code and CloudFormation templates generated automatically at compile time using C# source generators. The project is configured to use **Amazon.Lambda.Logging.AspNetCore** for CloudWatch logging and can be configured through both **`appsettings.json`** and environment variables.

*** 

### Local Development and Debugging

You have two primary options for local development and debugging:

#### 1. Running the Mock Lambda Test Tool

The project is configured to launch the **`dotnet-lambda-test-tool-8.0.exe`** from a launch profile in the **`launchSettings.json`** file. This tool provides a local host for running and testing your Lambda functions. The tool allows you to manually invoke your functions with different event payloads, which is useful for rapid, isolated testing.

#### 2. Using the SAM CLI

For a more comprehensive local emulation of the AWS environment, you can use the **AWS SAM CLI**. This is particularly useful for testing how your functions integrate with other AWS services, such as API Gateway.

1. **Install AWS CLI**: If you don't have it, follow the instructions on [Installing or updating to the latest version of the AWS CLI](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html).
2. **Install AWS SAM CLI**: Follow the instructions on [Installing the AWS SAM CLI](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/install-sam-cli.html).
3. **Build Project**: From the project directory, run the command **`sam build --template serverless.template`**. This will compile your application and prepare it for local execution.
4. **Start the Local API**: From the project directory, run the command **`sam local start-api`**. This will launch a local API Gateway endpoint that routes requests to your Lambda functions, as defined in the CloudFormation template.

***

### Configuration

Application configuration is handled using a standard .NET configuration provider, which reads from **`appsettings.json`** and environment variables. Environment variables take precedence over settings in the `appsettings.json` file, allowing you to easily manage different configurations for various environments (e.g., development, staging, production) without changing code.

***

### Deployment

To deploy the application to AWS, you'll use the **Amazon.Lambda.Tools** global tool.

1.  **Prerequisites**: 
    - If you don't have an AWS account, register at [AWS Console](https://aws.amazon.com/console/).
    - Ensure you have an IAM user with appropriate permissions and a configured AWS credentials file at `~/.aws/credentials`.
2.  **Install the Deployment Tool**: If not already installed, run `dotnet tool install --global Amazon.Lambda.Tools`.
3.  **Deploy**: From your project directory, execute the command **`dotnet lambda deploy-serverless`**. This command will package your application and deploy it to AWS as a CloudFormation stack.

***

### Resources

* **AWS Lambda Annotations GitHub Repository**: The official source code and detailed documentation for the framework.
    * [https://github.com/aws/aws-lambda-dotnet/blob/master/Libraries/src/Amazon.Lambda.Annotations/README.md](https://github.com/aws/aws-lambda-dotnet/blob/master/Libraries/src/Amazon.Lambda.Annotations/README.md)
* **AWS Extensions for .NET CLI**: Documentation for the deployment tools.
    * [https://github.com/aws/aws-extensions-for-dotnet-cli](https://github.com/aws/aws-extensions-for-dotnet-cli)
* **AWS Lambda for .NET**: The main GitHub repository for the AWS .NET Lambda tools.
    * [https://github.com/aws/aws-lambda-dotnet](https://github.com/aws/aws-lambda-dotnet)