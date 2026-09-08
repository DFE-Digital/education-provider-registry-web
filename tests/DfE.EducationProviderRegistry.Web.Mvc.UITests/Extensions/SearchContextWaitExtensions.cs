using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Web.Mvc.UITests.Extensions;

internal static class SearchContextWaitExtensions
{
    public static void Until<TSearchContext>(this IWait<TSearchContext> wait, Action<TSearchContext> action) where TSearchContext : ISearchContext
    {
        ArgumentNullException.ThrowIfNull(wait);
        ArgumentNullException.ThrowIfNull(action);

        wait.Until(driver =>
        {
            action(driver);
            return true;
        });
    }

    public static void ClickOn<TSearchContext>(this IWait<TSearchContext> wait, By locator)
        where TSearchContext : ISearchContext
    {
        wait.ClickOn(context => context.FindElement(locator));
    }

    public static void ClickOn<TSearchContext>(this IWait<TSearchContext> wait, Func<TSearchContext, IWebElement> finder)
        where TSearchContext : ISearchContext
    {
        ArgumentNullException.ThrowIfNull(wait);
        ArgumentNullException.ThrowIfNull(finder);

        wait.Until((context) =>
        {
            try
            {
                IWebElement element = finder(context);

                if (ElementIfVisibleOrDefault(element) is not null && ElementIfEnabledOrDefault(element) is not null)
                {
                    element.Click();
                    return true;
                }

                return false;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        });
    }


    public static IWebElement Find<TSearchContext>(this IWait<TSearchContext> wait, By by)
        where TSearchContext : ISearchContext
    {
        ArgumentNullException.ThrowIfNull(wait);
        ArgumentNullException.ThrowIfNull(by);

        return wait.Find(context => context.FindElement(by));
    }

    public static IWebElement Find<TSearchContext>(
        this IWait<TSearchContext> wait,
        Func<TSearchContext, IWebElement> finder)
            where TSearchContext : ISearchContext
    {
        ArgumentNullException.ThrowIfNull(wait);
        ArgumentNullException.ThrowIfNull(finder);

        return wait.Until(context =>
        {
            try
            {
                IWebElement element = finder(context);

                return ElementIfVisibleOrDefault(element);
            }
            catch (StaleElementReferenceException)
            {
                return null;
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        });
    }

    public static IReadOnlyCollection<IWebElement> FindMany<TSearchContext>(
        this IWait<TSearchContext> wait,
        By by)
        where TSearchContext : ISearchContext
    {
        ArgumentNullException.ThrowIfNull(wait);

        return wait.Until((context) =>
        {
            try
            {
                ReadOnlyCollection<IWebElement> located = context.FindElements(by);

                if (located.Count == 0)
                {
                    return null;
                }

                List<IWebElement> output = new(located.Count);

                for (int index = 0; index < located.Count; index++)
                {
                    IWebElement current = located[index];

                    // TODO visibility of element
                    output.Add(current);
                }

                // No visible results
                if (output.Count == 0)
                {

                    return null;
                }

                return output;
            }
            catch (StaleElementReferenceException)
            {
                return null;
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        });
    }

    private static IWebElement? ElementIfVisibleOrDefault(IWebElement? element)
    {
        if (element is null)
        {
            return null;
        }

        return element.Displayed ? element : null;
    }

    private static IWebElement? ElementIfEnabledOrDefault(IWebElement? element)
    {
        if (element is null)
        {
            return null;
        }

        return element.Enabled ? element : null;
    }
}
