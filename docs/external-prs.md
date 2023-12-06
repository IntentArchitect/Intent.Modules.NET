# External Pull Request (PR) Process

Welcome to the External Pull Request (PR) process for Intent Architect modules. We appreciate and encourage our customers to contribute by submitting pull requests for desired changes in our modules. The following outlines the process for submitting, reviewing, and releasing pull requests.

## 1. Pull Request Submission

We welcome pull requests from anyone for any Intent Architect module, proposing changes they would like to see implemented.

### Contribution Guidelines

Changes should adhere to the following principles:

- Is in line with accepted coding practices in the solution space.
- Is a general pattern which other developers would want to use.
- Generated code should always read as if a developer wrote the code.

## 2. Pull Request Review

Pull requests will be reviewed by our team to ensure it meets the above mentioned guidelines.

As our team will be ultimately responsibile for maintaining the module going forward, we will do any additional work required to bring the change in-line with our internal best practices and standards:

- Generalization of code (if required)
- Release Versioning
- Creation of Test Cases, Testing amd Integration with other Intent Architect modules

Once the review is completed, the approved change is merged into our Development branch, and a pre-release version is published.

## 3. Pre-release published

A pre-release version of the module, incorporating the proposed change, is made available. This allows contributors and other stakeholders to thoroughly test and validate the functionality of the module.

Pre-releases will be versioned as follows: "{Expected Official release version number}-pre.*"
If the change was to a module name `Intent.SomeModule` which was currently at version `1.2.0`, and the change is considered to be a `patch`, then you could expect the following:

- Pre release(s) : v 1.2.1-pre.*
- Official release : v 1.2.1

## 4. Official release

Assuming no issues arise during the pre-release testing phase, the pre-release version is promoted to an official release typically within a maximum of two weeks. This ensures a timely transition from pre-release to a stable and officially endorsed version.

We value your contributions and look forward to maintaining the quality and integrity of Intent Architect modules through this collaborative process. Thank you for your commitment to excellence!
