# Running and building and debugging

## How to build playwright tests

Navigate to the playwright project, located within the _tests_ folder

```
cd tests/Dfe.ManageEntitlementFunding.Tests.E2e
```

and build with

```
dotnet build
```
## How to run playwright tests

Navigate to the playwright project, located within the _tests_ folder

```
cd tests/Dfe.ManageEntitlementFunding.Tests.E2e
```

and run with

```
dotnet test
```

## How to debug through a browser UI

```
PWDEBUG=1 dotnet test
```

## How to specify particular tests

If you have a test where the method is called `PerformLogin()`, then you can run or debug just this one test by adding the flag
`--filter "PerformLogin"`

e.g. 
```
dotnet test --filter "PerformLogin"
```

# Writing tests
