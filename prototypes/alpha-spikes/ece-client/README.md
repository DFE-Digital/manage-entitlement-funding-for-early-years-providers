# ECE PoC

.NET 8 console application demonstrating integration with the DfE Eligibility Checking Engine.

This code was developed during Alpha to explore integration with the ECE eligibility service.

It demonstrates feasibility but should not be regarded as production code or a reference
implementation.

## Supported checks
- Working Families
- Two Year Offer

Update the project secrets with your API credentials and run:

```bash
dotnet run -- wf 90246760112 2019-05-15 Johnson AB123456C
```

or

```bash
dotnet run -- 2y 2022-06-07 Williams NN124578A
```

(These correspond to records in the ECE test system's data set.)

For development, it is recommended that you put your credentials for the ECE API into secrets. This
can be done on the command line, from the folder containing the `*.csproj` file. The project file
is already set up with a secrets ID, so you should not need to initialise the secrets feature for
the project.

```bash
dotnet user-secrets set "Ece:Username" "<my-assigned-user>"
dotnet user-secrets set "Ece:Password" "<my-api-secret>"
```
