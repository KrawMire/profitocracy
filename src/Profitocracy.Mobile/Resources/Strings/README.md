# Add more languages

1. Create a new "Resources" file, e.g. `AppResources.[language-code].resx`
2. Delete the automatically generated file `AppResources.[language-code].Designer.cs`
3. Open the file [Profitocracy.Mobile.csproj](../../Profitocracy.Mobile.csproj) and remove the following entries:

```xml
<ItemGroup>
    <EmbeddedResource Update="Resources\Strings\AppResources.[language-code].resx">
        <Generator>ResXFileCodeGenerator</Generator>
        <LastGenOutput>AppResources.[language-code].Designer.cs</LastGenOutput>
    </EmbeddedResource>
</ItemGroup>
```

and

```xml
<ItemGroup>
    <Compile Update="Resources\Strings\AppResources.[language-code].Designer.cs">
        <DesignTime>True</DesignTime>
        <AutoGen>True</AutoGen>
        <DependentUpon>AppResources.[language-code].resx</DependentUpon>
    </Compile>
</ItemGroup>
```
