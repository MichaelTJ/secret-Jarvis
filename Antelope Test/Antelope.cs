using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml;
using Calais;
using IKVM;
using ikvm;
using jena;
using Jena;
using Proxem.NanoProlog;
using Proxem.ShortestPath;
using Proxem.Antelope.Coreferences;
using Proxem.Antelope.Disambiguation;
using Proxem.Antelope.Features;
using Proxem.Antelope.Formatters;
using Proxem.Antelope.Lexicon;
using Proxem.Antelope.LinkGrammar;
using Proxem.Antelope.Paraphrases;
using Proxem.Antelope.Parsing;
using Proxem.Antelope.Predicates;
using Proxem.Antelope.Semantics;
using Proxem.Antelope.Stanford;
using Proxem.Antelope.Tagging;
using Proxem.Antelope.Tagmatica;
using Proxem.Antelope.Tools;
using Proxem.Antelope.WinAPI;
using StanfordParser;

namespace Antelope_Test
{
    /// <summary>
    /// Antelope Summary is here
    /// </summary>
    class Antelope
    {
        ITagger simpleTagger;
        public ILexicon lexicon;

        public Antelope()
        {
            Console.WriteLine("Loading Simple Tagger...");
            string simpleTaggerFile = @"BrillTaggerLexicon.txt";
            simpleTagger = new SimpleTagger(simpleTaggerFile);

            Console.WriteLine("Loading Lexicon...");
            lexicon = new Lexicon();
            lexicon.LoadDataFromFile("Proxem.Lexicon.dat", null);

            whichDef();
        }

        public IList<IWord> ITag(string sentence)
        {
            return simpleTagger.TagText(sentence);
        }

        public void LexiconExamples()
        {

            ILexicon lexicon = new Lexicon();
            lexicon.LoadDataFromFile("Proxem.Lexicon.dat", null);


        }
        public void ISynsetExamples()
        {

            ILexicon lexicon = new Lexicon();
            lexicon.LoadDataFromFile("Proxem.Lexicon.dat", null);

            //Senses of Cat
            IList<ILemma> senses = lexicon.FindSenses("cat", Proxem.Antelope.PartOfSpeech.Noun);
            foreach (ILemma s in senses)
            {
                Console.WriteLine("Annotations: {0}", s.Annotations);
                Console.WriteLine("Frequency: {0}", s.Frequency);
                Console.WriteLine("Language: {0}", s.Language);
                Console.WriteLine("Lemma: {0}", s.Lemma);
                Console.WriteLine("PartOfSpeech: {0}", s.PartOfSpeech);
                Console.WriteLine("Relations: {0}", s.Relations);
                Console.WriteLine("SenseNo: {0}", s.SenseNo);
                Console.WriteLine("Synset: {0}", s.Synset);
                Console.WriteLine("SynsetId: {0}", s.SynsetId);
                Console.WriteLine("Text: {0}", s.Text);
                Console.WriteLine("WNLexId: {0}", s.WNLexId);
                Console.WriteLine("WordNo: {0}", s.WordNo);
                Console.WriteLine();
            }

            //Definitions of synsets
            Console.WriteLine("True cat definition: ");
            ISynset trueCat = senses[3].Synset;
            Console.WriteLine(trueCat.Definition);

            //Related synset Ancestors
            Console.WriteLine("True cat ancestors: ");
            IList<ISynset> trueCatDirectAncestors = trueCat.RelatedSynsets(RelationType.Hypernym);
            foreach (ISynset syn in trueCatDirectAncestors)
            {
                Console.WriteLine(syn.Lemma.Text);
            }

            //Related synset Descendants
            Console.WriteLine("True cat descendants: ");
            IList<ISynset> trueCatDirectDescendants = trueCat.RelatedSynsets(RelationType.Hyponym);
            foreach (ISynset syn in trueCatDirectDescendants)
            {
                Console.WriteLine(syn.Lemma.Text);
            }

            //Related sysnset AllAncestors
            Console.WriteLine("All Ancestors: ");
            IList<ISynset> trueCatDirectAllAscendants = trueCat.RelatedSynsets(RelationType.Hyponym);
            foreach (ISynset syn in trueCatDirectAllAscendants)
            {
                Console.WriteLine(syn.Lemma.Text);
            }

            //Related synsets AllDescendants
            Console.WriteLine("All Descendants: ");
            IList<ISynset> trueCatDirectAllDescendants = trueCat.RelatedSynsets(RelationType.Hyponym);
            foreach (ISynset syn in trueCatDirectAllDescendants)
            {
                Console.WriteLine(syn.Lemma.Text);
            }

            //How Similar are two sets? (Mutual Information Content)
            Console.WriteLine("Similar jaguar: ");
            ILemma jaguar = lexicon.FindSenses("jaguar", Proxem.Antelope.PartOfSpeech.Noun)[0];
            double similarJag = trueCat.Lemma.GetSimilarity(jaguar, SimilarityMeasure.MutualInformationContent);
            Console.WriteLine(similarJag);

            //How Similar are two sets? (Gloss Overlapping)
            Console.WriteLine("Similar sets?");
            ILemma jaguar2 = lexicon.FindSenses("jaguar", Proxem.Antelope.PartOfSpeech.Noun)[0];
            double similarJag2 = trueCat.Lemma.GetSimilarity(jaguar, SimilarityMeasure.GlossOverlapping);
            Console.WriteLine(similarJag2);

            //Direct "parts" of a "motorcar"?
            Console.WriteLine("SImilar part, motorcar: ");
            ISynset car = lexicon.FindSenses("motorcar", Proxem.Antelope.PartOfSpeech.Noun)[0].Synset;
            IList<ISynset> carParts = car.RelatedSynsets(RelationType.HasPart);
            foreach (ISynset carPart in carParts)
            {
                Console.WriteLine(carPart.Lemma.Text);
            }
        }
        public void ITaggerExamples()
        {
            //Advanced Tagger
            //Simple Tagger
            string simpleTaggerFile = @"BrillTaggerLexicon.txt";
            ITagger simpleTagger = new SimpleTagger(simpleTaggerFile);

            //Tagging a string
            const string sentence = "Born in a Kentucky log cabin, Abraham Lincoln was elected as President of the United States";
            IList<IWord> words = simpleTagger.TagText(sentence);
            Console.WriteLine(words.Count);
            foreach (IWord w in words)
            {
                Console.WriteLine("Word: " + w);
                Console.WriteLine("Annotations: " + w.Annotations);
                Console.WriteLine("BaseForms: " + w.BaseForms);
                Console.WriteLine("IndexInSentence: " + w.IndexInSentence);
                Console.WriteLine("Language: " + w.Language);
                Console.WriteLine("PartOfSpeech: " + w.PartOfSpeech);
                Console.WriteLine("Senses: " + w.Senses);
                Console.WriteLine("Subwords: " + w.Subwords);
                Console.WriteLine("Tag: " + w.Tag);
                Console.WriteLine("TagAsString: " + w.TagAsString);
                Console.WriteLine("Text: " + w.Text);
                Console.WriteLine("Tokens: " + w.Tokens);
                Console.WriteLine();
            }

            //Cloning Words
            //IList<IWord> wordsCopy = 
            //Doesn't work

            //Collapsing collacations
            /*
            ILexicon lexicon = new Lexicon();
            lexicon.LoadDataFromFile("Proxem.Lexicon.dat", null);
            IList<IWord> collapsedWords = simpleTagger.CollapseCollocations(words, lexicon);
            */



        }
        public void IChunkerExamples()
        {
            //List of tagged words as input... new list chunk nodes
            //Simple Tagger
            string simpleTaggerFile = @"BrillTaggerLexicon.txt";
            ITagger simpleTagger3 = new SimpleTagger(simpleTaggerFile);

            //Tagging a string
            const string sentence = "Born in a Kentucky log cabin, Abraham Lincoln was elected as President of the United States";
            IList<IWord> words = simpleTagger3.TagText(sentence);

            //Collapsing collacations
            ILexicon lexi = new Lexicon();
            lexi.LoadDataFromFile("Proxem.Lexicon.dat", null);
            //IList<IWord> collapsedWords = simpleTagger3.CollapseCollocations(words, lexi);
        }
        public void whichDef()
        {
            Console.WriteLine("Ready");
            //Get the sentence
            string sentence = "The cat runs.";

            //Tokenize
            IList<IWord> words = simpleTagger.TagText(sentence);
            //For each word in sentence
            foreach (IWord Iw in words)
            {

                Console.WriteLine("NEW WORD" + Iw.Text);

                if (Iw.PartOfSpeech == Proxem.Antelope.PartOfSpeech.Noun)
                {
                    //Find all senses of the word
                    IList<ILemma> senses = lexicon.FindSenses(Iw.Text, Iw.PartOfSpeech);
                    
                    //Init value for bestFit result
                    double bestWordSimTotal = 0;
                    //foreach sense
                    foreach (ILemma sense in senses)
                    {
                        double wordSimTotal = 0;
                        //for each word in the sense
                        foreach (IWord IWord in words)
                        {
                            Console.WriteLine("Iw.text: " + IWord.Text);
                            Console.WriteLine("Iw.PartOfSpeech: " + IWord.PartOfSpeech);
                            IList<ILemma> FirstDefWords = lexicon.FindSenses(IWord.Text, IWord.PartOfSpeech);
                            foreach (ILemma FirstWordDefs in FirstDefWords)
                            {
                                Console.Write("IWord def: ");
                                Console.WriteLine(FirstWordDefs.Text);
                            }
                            try
                            {
                                wordSimTotal += sense.Lemma.GetSimilarity(FirstDefWords[0], SimilarityMeasure.GlossOverlapping);
                            }
                            catch { }
                        }
                        if (wordSimTotal > bestWordSimTotal)
                        {
                            bestWordSimTotal = wordSimTotal;
                            Console.WriteLine(wordSimTotal);
                            wordSimTotal = 0;
                            Console.WriteLine(sense.Lemma.Synset.Definition);
                            Console.WriteLine();
                        }
                    }
                }
            }
        }

        private void match(string incoming)
        {
            List<string> knownStrings = new List<string>();
            knownStrings.Add("Hello world!");
            knownStrings.Add("Create a list.");
            knownStrings.Add("Your mum sucks!");
            knownStrings.Add("Adds an element to the list");
            knownStrings.Add("Increases a number by one.");

            //Tag the incoming string
            IList<IWord> taggedIncoming = ITag(incoming);

            //Tag each knownString
            Dictionary<string, IList<IWord>> taggedKnowns = new Dictionary<string, IList<IWord>>();
            foreach(string str in knownStrings)
            {
                taggedKnowns.Add(str, ITag(str));
                foreach(IWord word in taggedKnowns[str])
                {
                    //Needed to compare senses
                    word.ComputeSenses(lexicon);
                }
                
            }


            //Now to compare the incoming string with the known strings
            //foreach of the known strings
            foreach(IList<IWord> tK in taggedKnowns.Values)
            { 

            }
        }
    }
}
