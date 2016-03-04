using System.Collections.Generic;
using System.Xml.Linq;
using AspectDotNet;
using Orchard;
using Orchard.FileSystems.Media;
using Orchard.Recipes.Services;
namespace Aspect2
{

    public class WriteInJournal : Aspect
    {
       public static IOrchardServices Services { get; set; }
       private static IStorageProvider _storageProvider;

       //конструктор не внедряется
      // [AspectAction("%after %call *RecipeJournalManager.ctor(IStorageProvider) && args(..)")]
        public static void GetParameter(IStorageProvider storageProvider) {
            _storageProvider = storageProvider;
        }
       
        //
        public static dynamic Method(string executionId) {
            var obj = new RecipeJournalManager(_storageProvider);
            var executionJournal = obj.GetJournalFile(executionId);
            var xElement = XElement.Parse(RecipeJournalManager.ReadJournal(executionJournal));
            var dic = new Dictionary<string, dynamic> { { "executionJournal", executionJournal }, { "xElement", xElement } };
            return dic;
        }
      
       //метод внедряется
       // [AspectAction("%instead %call *IRecipeJournal.ExecutionStart(string) && args(..)")]
        public static void ExecutionStart(string executionId) 
        {
            var dic = Method(executionId);
            dic["xElement"].Element("Status").Value = "Started";
            RecipeJournalManager.WriteJournal(dic["executionJournal"], dic["xElement"]);
        }

        //метод внедряется
        //[AspectAction("%instead %call *IRecipeJournal.ExecutionComplete(string) && args(..)")]
        public static void ExecutionComplete(string executionId)
        {
            var dic = Method(executionId);
            dic["xElement"].Element("Status").Value = "Complete";
            RecipeJournalManager.WriteJournal(dic["executionJournal"], dic["xElement"]);
        }

        //метод внедряется
        //[AspectAction("%instead %call *IRecipeJournal.ExecutionFailed(string) && args(..)")]
        public static void ExecutionFailed(string executionId)
        {
            var dic = Method(executionId);
            dic["xElement"].Element("Status").Value = "Failed";
            RecipeJournalManager.WriteJournal(dic["executionJournal"], dic["xElement"]);
        }

        //метод внедряется
        //[AspectAction("%instead %call *IRecipeJournal.WriteJournalEntry(string, string) && args(..)")]
        public static void WriteJournalEntry(string executionId, string message)
        {
            var dic = Method(executionId);
            var journalEntry = new XElement("Message", message);
            dic["xElement"].Add(journalEntry);
            RecipeJournalManager.WriteJournal(dic["executionJournal"], dic["xElement"]);
        }  
    }
}