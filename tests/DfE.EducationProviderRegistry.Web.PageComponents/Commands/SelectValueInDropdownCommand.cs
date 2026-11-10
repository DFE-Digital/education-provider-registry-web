using System;
using System.Collections.Generic;
using System.Text;
using DfE.Document.Abstractions;

namespace DfE.EducationProviderRegistry.Web.PageComponents.Commands;

public sealed class SelectValueInDropdownCommand : ICommand<SelectValueInDropdownCommandModel>
{
}

public sealed record class SelectValueInDropdownCommandModel
{
    public required string Selector { get; init; }
    public required string Value { get; init; }

}
