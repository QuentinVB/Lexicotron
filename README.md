# Lexicotron

This C# project is linked to the work of [Agathe Delepaut](https://www.linkedin.com/in/agathe-delepaut/) and his dissertation on "lexical field usage in public article about osteopathy"
It uses the french [lexic3.83](http://www.lexique.org) database of words, the made up by her, lexical field index and the [babelNET](https://babelnet.org/) api.

To store words info from babel, the project embed an [SQlite](https://sqlite.org/index.html) database using [Dapper](https://dapper-tutorial.net/dapper).

It produces excel files via the [Office Interop DLL](https://docs.microsoft.com/fr-fr/dotnet/api/microsoft.office.interop.excel?view=excel-pia) to produces word frequency analysis

## project architecture

The solution contain multiple project

|                     | Usage |
|---------------------|-------|
| Lexicotron.Core     | load txt articles and process them using ressources |
| Lexicotron.Database | provide connexion to sqlite database, csv and excel files      |
| Lexicotron.BabelAPI | provide modesl, connexion to the babelNET api      |
| Lexicotron.Tests    | provide UnitTests using VS tests platform      |
| Lexicotron.UI       | main entry point, provide user input, logging      |

## Required ressources

- a CSV file of the lexic3.83 (for faster loading, it should use the excel version)
- an excel file with the association Lexical Field <-> Words collection
- a txt file with the babelNET API Key

## Analyse restriction

To ease the analyse of the press article, each word will be converted to his lemme form and each verb to the infinitive version

## Limits

Due to BabelNET API limit (1000 daily requests), the program request each day words and relation and store them in a local sqlite database.
But some word exceed the capacity of the api caller. For instance the word *human* reach the timeout limit of http client. (the returned json result reach 3.4Go !!)
A solution could be found using http://www.tugberkugurlu.com/archive/efficiently-streaming-large-http-responses-with-httpclient

## External Documentation

https://anceret-matthieu.developpez.com/tutoriels/utiliser-sqlite-a-traversnet/

https://joshclose.github.io/CsvHelper/

https://ef.readthedocs.io/en/staging/platforms/netcore/new-db-sqlite.html

https://stackify.com/cross-platform-net-core-apps/

https://docs.microsoft.com/fr-fr/dotnet/standard/io/how-to-read-text-from-a-file

https://docs.microsoft.com/fr-fr/dotnet/api/system.io.stream?view=netframework-4.8

http://csharp.net-informations.com/excel/csharp-read-excel.htm

https://docs.microsoft.com/fr-fr/dotnet/api/microsoft.office.interop.excel?view=excel-pia

https://app.quicktype.io

https://babelnet.org/guide
