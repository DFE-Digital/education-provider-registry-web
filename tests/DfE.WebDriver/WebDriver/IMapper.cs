namespace DfE.WebDriver.WebDriver;

internal interface IMapper<in TIn, out TOut>
{
    TOut Map(TIn input);
}
