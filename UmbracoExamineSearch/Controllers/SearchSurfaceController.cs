using Examine;
using Examine.SearchCriteria;
using System.Collections;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using UmbracoExamineSearch.Models;

namespace UmbracoExamineSearch.Controllers
{
    public class SearchSurfaceController : SurfaceController
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(SearchModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Error!";
                return CurrentUmbracoPage();
            }

            // creating custom searcher
            var searcher = ExamineManager.Instance.SearchProviderCollection["ExternalSearcher"];

            // Create search criteria to build the search query
            var searchCriteria = searcher.CreateSearchCriteria(BooleanOperation.Or);

            // var query = this.SearchByTwoFields(searchCriteria, "hello");
            // var query = GetThisPagesWithThisDocTypeAndMustHaveThisFieldValue(searchCriteria, "Home", "hello");
            // var query = GetThisPagesWithThisDocTypeOrAllOtherPagesThatHaveThisFieldValueInContentField(searchCriteria, "Home", "hello");
            // var query = GetThisPagesWithThisDocTypeAndWithThisFieldsMustContainThisValue(searchCriteria, "Test", "hello");
            var query = GetThisPagesWithThisDocTypeOrPagesWithSomeOfThisFieldsThatContainThisValue(searchCriteria, "Test", "hello");

            var results = searcher.Search(query);

            // Put results in view bag and throw it to view
            ViewBag.SearchResults = results;

            TempData["Success"] = "Results!";
            return CurrentUmbracoPage();
        }

        private static ISearchCriteria GetThisPagesWithThisDocTypeOrPagesWithSomeOfThisFieldsThatContainThisValue(ISearchCriteria searchCriteria, string docTypeAlias, string fieldValue)
        {
            // LuceneQuery: nodeTypeAlias:test (testTitle:hello testDescription:hello)
            // Means give me this pages from doc type "Home" or pages with some of this fields "testTitle" and "content", must contains word "hello"

            var query = searchCriteria
                .Field("nodeTypeAlias", docTypeAlias)
                .Or()
                .GroupedOr(new string[] { "testTitle", "content" }, new string[] { fieldValue })
                .Compile();
            return query;
        }

        private static ISearchCriteria GetThisPagesWithThisDocTypeAndWithThisFieldsMustContainThisValue(ISearchCriteria searchCriteria, string docTypeAlias, string fieldValue)
        {
            // LuceneQuery: nodeTypeAlias:test +(+testTitle:hello +testDescription:hello)
            // Means give me this pages from doc type "Home" and they fields "testTitle" and "testDescription" must contains word "hello"

            var query = searchCriteria
                .Field("nodeTypeAlias", docTypeAlias)
                .And()
                .GroupedAnd(new string[] { "testTitle", "testDescription" }, new string[] { fieldValue })
                .Compile();
            return query;
        }

        private static ISearchCriteria GetThisPagesWithThisDocTypeOrAllOtherPagesThatHaveThisFieldValueInContentField(ISearchCriteria searchCriteria, string docTypeAlias, string fieldValue)
        {
            // LuceneQuery: {nodeTypeAlias:home content:hello}
            // Means - give me only this pages that is from "Home" doc type or any other pages that have word "hello" in the "content" property

            var query = searchCriteria
                .Field("nodeTypeAlias", docTypeAlias)
                .Or()
                .Field("content", fieldValue)
                .Compile();
            return query;
        }

        private static ISearchCriteria GetThisPagesWithThisDocTypeAndMustHaveThisFieldValue(ISearchCriteria searchCriteria, string docTypeAlias, string fieldValue)
        {
            // LuceneQuery: {nodeTypeAlias:home +content:hello}
            // Means - give me only this pages that is from "Home" doc type and have word "hello" in the "content" property

            var query = searchCriteria
                .Field("nodeTypeAlias", docTypeAlias)
                .And()
                .Field("content", fieldValue)
                .Compile();
            return query;
        }

        private ISearchCriteria SearchByTwoFields(ISearchCriteria searchCriteria, string fieldValue)
        {
            // LuceneQuery: {testTitle:hello testDescription:hello}
            // Means give me all pages that contains "hello" in it's "testTitle" field of contains the same word in it's "testDescription" field

            var query = searchCriteria
                .Field("testTitle", fieldValue)
                .Or()
                .Field("testDescription", fieldValue)
                .Compile();
            return query;
        }

        private IEnumerable SearchInAllData(string searchedText)
        {
            var results = Umbraco.TypedSearch(searchedText);
            return results;
        }
    }
}