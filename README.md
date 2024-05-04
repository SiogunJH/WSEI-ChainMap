# Zadanie: `ChainMap` - implementacja w C#

  > Krzysztof Molenda, 2024-05-04

## Wprowadzenie

W języku Python, w module `collections`, znajduje się klasa `ChainMap`, która pozwala na łączenie wielu słowników w jeden. W przypadku konfliktu kluczy, wartość z pierwszego słownika jest zwracana. W C# nie ma wbudowanej implementacji `ChainMap`, ale można ją zaimplementować samodzielnie - jako ćwiczenie.

Do czego taka struktura danych może się przydać? 

Rozważmy następujący przykładowy scenariusz. Użytkownik korzysta ze skomplikowanej aplikacji. Aplikacja ta ma wiele konfiguracji, które mogą być domyślnie zdefiniowane w różnych źródłach (słownikach: mapowaniu klucz-wartość). Jedna z konfiguracji dotyczy modułu aplikacji `A`, a druga modułu `B`. Uzytkownik widzi - za pośrednictwem `ChainMap` - jedną konfigurację - sumę konfiguracji z modułów `A` i `B`, ale wewnętrznie są to dwa słowniki, które są łączone w jedną strukturę danych. W przypadku konfliktu kluczy, wartość z konfiguracji modułu `A` ma priorytet, ponieważ dodana została jako pierwsza (jest zatem ważniejsza).

Dodatkowo, użytkownik może chcieć wprowadzić własne ustawienia, które przesłaniają domyślne. Wtedy dokonuje wpisów w wyodrębnionym **słowniku głównym** struktury, który traktowany jest z najwyższym priorytetem. W ten sposób, użytkownik może dostosować konfigurację aplikacji do swoich potrzeb, bez ingerencji w ustawienia konfiguracji domyślnych modułów `A` i `B`. Usunięcie własnych wpisów powoduje powrót do konfiguracji domyślnej. Oczywiście może również dodawać, usuwać i modyfikować własne wpisy, których nie ma w konfiguracjach domyślnych.


## Specyfikacja struktury danych `ChainMap` w C#

* `ChainMap<TKey, TValue>` grupuje wiele słowników w jedną strukturę danych, choć z zewnątrz postrzegana jest jako jeden słownik. Technicznie jest listą słowników, a dostęp do nich jest realizowany w kolejności od pierwszego do ostatniego. 

* Struktura inicjowana jest konstruktorem, w którym przekazywane są słowniki, które mają zostać połączone. Kolejność przekazania słowników jest ważna - wcześniejszy na liście ma wyższy priorytet. 

* Zawsze tworzony jest słownik główny - pierwszy na liście, o najwyższym priorytecie, edytowalny, początkowo pusty. Jeśli w konstruktorze nie zostanie przekazany żaden słownik, tworzony jest tylko słownik główny.

* Słowniki poza głównym są niemodyfikowalne, tylko do odczytu.

* Struktura danych powinna umożliwiać dodawanie nowych słowników, jak również ich usuwanie z listy, ale bez pierwszego (głównego). Dodając nowy słownik należy określić jego położenie na liście, określające jednocześnie jego priorytet.

* Operowanie na `ChainMap` powinno być takie samo jak na słowniku `Dictionary<TKey, TValue>` - możemy dodawać, usuwać i modyfikować wartości na podstawie klucza - ale wyłącznie w słowniku głównym. Odczyt wartości w pierwszej kolejności jest realizowany w słowniku głównym, a jeśli jej tam nie ma, to poszukiwana jest ona w kolejnych słownikach (w kolejności zapisania na liście) i zwracana (tylko do odczytu - słowniki poza głównym są niemodyfikowalne). Modyfikacja wartości dla zadanego klucza, jeśli tej wartości nie ma w słowniku głównym, a jest w jednym z kolejnych słowników, powinna być zapisana w słowniku głównym. Usunięcie wartości dla zadanego klucza powinno być realizowane w słowniku głównym, a jeśli tej wartości nie ma w słowniku głównym, to operacja nie jest wykonywana, bez zgłoszenia wyjątku.

* `ChainMap` powinien implementować interfejs `IDictionary<TKey, TValue>`, ale z ograniczeniami - nie wszystkie metody interfejsu powinny być w pełni dostępne.
    - `Add`  - dodaje wpis do słownika głównego, jeśli klucz w nim nie istnieje. Jeśli klucz istnieje w słowniku głównym, zgłasza wyjątek `ArgumentException`. Jeśli klucz istnieje w jednym z kolejnych słowników, to dodaje wpis do słownika głównego. Nie modyfikuje słowników dołączonych.
    - TryAdd - dodaje wpis do słownika głównego, jeśli klucz w nim nie istnieje. Jeśli klucz istnieje w słowniku głównym, zwraca `false`. Jeśli klucz istnieje w jednym z kolejnych słowników, to dodaje wpis do słownika głównego i zwraca `true`. Nie modyfikuje słowników dołączonych.
    - `Remove` - usuwa wpis z słownika głównego, jeśli klucz w nim istnieje i zwraca `true`. Jeśli klucz nie istnieje w słowniku głównym, zwraca `false`. Nie modyfikuje słowników dołączonych.
    - `TryGetValue` - zwraca wartość dla zadanego klucza, jeśli istnieje w słowniku głównym. Jeśli klucz nie istnieje w słowniku głównym, zwraca wartość z pierwszego słownika, w którym klucz istnieje (zgodnie z priorytetem na liście). Jeśli klucz nie istnieje w żadnym słowniku, zwraca `false`.
    - `ContainsKey` - zwraca `true`, jeśli klucz istnieje w jakimkolwiek słowniku. W przeciwnym razie, zwraca `false`.
    - ContainsValue - zwraca `true`, jeśli wartość istnieje w jakimkolwiek słowniku. W przeciwnym razie, zwraca `false`.
    - `Keys` - zwraca wszystkie klucze ze wszystkich słowników.
    - `Values` - zwraca wszystkie wartości ze wszystkich słowników.
    - `Count` - zwraca liczbę wszystkich wpisów ze wszystkich słowników.
    - `IsReadOnly` - zwraca `false`.
    - `this[]` - indexer, który umożliwia dostęp do wartości na podstawie klucza. Operacja `get` - jeśli klucz istnieje w słowniku głównym, zwraca wartość z tego słownika. W przeciwnym razie, zwraca wartość z pierwszego słownika, w którym klucz istnieje (zgodnie z priorytetem na liście). Jeśli klucz nie istnieje w żadnym słowniku, zgłasza wyjątek `KeyNotFoundException`. Operacja `set` zapisuje wartość w słowniku głównym, jeśli klucz istnieje w nim. Jeśli klucz nie istnieje w słowniku głównym, ale istnieje w jednym z kolejnych słowników, to zapisuje wartość w słowniku głównym. Operacja `set` nie modyfikuje słowników dołączonych.
    - `Clear()` - usuwa wszystkie wpisy ze słownika głównego, ale nie modyfikuje słowników dołączonych.
    - `GetEnumerator()` - zwraca enumerator dla wszystkich wpisów ze wszystkich słowników.

* Zarządzanie słownikami połączonymi:

    - `AddDictionary(IDictionary<TKey, TValue> dictionary, int index)` - dodaje słownik do listy słowników połączonych. Słownik dodawany jest na pozycji `index`. Jeśli `index` jest mniejszy od 0, słownik dodawany jest na końcu listy. Jeśli `index` jest większy od liczby słowników, słownik dodawany jest na początku listy. Słownik dodawany jest jako niemodyfikowalny, tylko do odczytu.
    - `RemoveDictionary(int index)` - usuwa słownik z listy słowników połączonych. Jeśli indeks jest poza zakresem, nie wykonuje operacji i nie zgłasza wyjątku.
    - `ClearDictionaries()` - usuwa wszystkie słowniki z listy słowników połączonych.
    - `CountDictionaries` - zwraca liczbę słowników połączonych.
    - `GetDictionaries()` - zwraca listę słowników połączonych jako niemodyfikowalną, tylko do odczytu.
    - `GetDictionary(int index)` - zwraca słownik o podanym indeksie z listy słowników połączonych. Słownik zwracany jest jako niemodyfikowalny, tylko do odczytu.
    - `GetMainDictionary()` - zwraca słownik główny jako modyfikowalny.
    - `Merge()` - zwraca `ChainMap` jako nowy słownik, w którym wszystkie słowniki są połączone w jedną strukturę danych typu `Dictionary<TKey, TValue>`. W przypadku konfliktu kluczy, wartość z pierwszego słownika (według priorytetu) jest zwracana. Wartości przyporządkowane kluczom w słownikach połączonych są kopiowane do nowego słownika.

## Przykład użycia

```csharp
using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        var dict1 = new Dictionary<string, string>
        {
            { "a", "1" },
            { "b", "2" },
            { "c", "3" }
        };

        var dict2 = new Dictionary<string, string>
        {
            { "b", "22" },
            { "c", "33" },
            { "d", "44" }
        };

        var dict3 = new Dictionary<string, string>
        {
            { "c", "333" },
            { "d", "444" },
            { "e", "555" }
        };

        var chainMap = new ChainMap<string, string>(dict1, dict2, dict3);

        Console.WriteLine(chainMap["a"]); // 1
        Console.WriteLine(chainMap["b"]); // 2
        Console.WriteLine(chainMap["c"]); // 3
        Console.WriteLine(chainMap["d"]); // 44
        Console.WriteLine(chainMap["e"]); // 555

        // add to main dictionary
        chainMap["a"] = "11";
        chainMap["b"] = "22";
        chainMap["c"] = "33";
        chainMap["d"] = "44";
        chainMap["e"] = "55";

        Console.WriteLine(chainMap["a"]); // 11
        Console.WriteLine(chainMap["b"]); // 22
        Console.WriteLine(chainMap["c"]); // 33
        Console.WriteLine(chainMap["d"]); // 44
        Console.WriteLine(chainMap["e"]); // 55

        // remove from main dictionary
        chainMap.Remove("a");
        Console.WriteLine(chainMap["a"])); // 1

        chainMap.Add("f", "66");
        Console.WriteLine(chainMap["f"]); // 66

        chainMap.Remove("f");
        Console.WriteLine(chainMap.ContainsKey("f")); // False

        chainMap.AddDictionary(new Dictionary<string, string> { { "g", "77" } }, 0);
        Console.WriteLine(chainMap["g"]); // 77

        chainMap.RemoveDictionary(0);
        Console.WriteLine(chainMap.ContainsKey("g")); // False

        chainMap.ClearDictionaries();
        Console.WriteLine(chainMap.Count); // 0
    }
}
```

## Zadanie

Opracuj implementację struktury danych `ChainMap` w języku C# zgodnie z powyższą specyfikacją. Opracuj testy jednostkowe, które sprawdzą poprawność działania tej struktury danych. Postaraj się pokryć 100% kodu przypadkami testowymi.

Przygotuj przykłady użycia struktury danych `ChainMap` w różnych scenariuszach, które pokazują jej pełną funkcjonalność.

Do oceny przesyłasz skompresowane *solution* w Visual Studio lub VS Code, bez folderów `/bin` oraz `/obj`, składające się z 3 projektów:

* projektu typu biblioteka klas `ChainMapLib` z implementacją struktury danych `ChainMap`,
* projektu typu testy jednostkowe `ChainMapTests` z testami jednostkowymi dla struktury danych `ChainMap`,
* projektu konsolowego `ChainMapApp` z przykładami użycia struktury danych `ChainMap`.

Jesli jakieś z wymagań nie zostało dokładnie opisane, jest niejasne lub nawet błędne, to proszę o interpretację i zaimplementowanie zgodnie z własnym rozumieniem. W komentarzu do zadania proszę o krótkie uzasadnienie wyboru rozwiązania.

⚠️ Zastrzegam sobie prawo do weryfikacji samodzielności wykonania zadania. 

## Referencje

* https://docs.python.org/3/library/collections.html#collections.ChainMap

* [Python's ChainMap: Manage Multiple Contexts Effectively](https://realpython.com/python-chainmap/)

* [Python ChainMap In Collections Tutorial – Complete Guide](https://gamedevacademy.org/python-chainmap-in-collections-tutorial-complete-guide/)

* [C#: FrozenDictionary<TKey,TValue> Class](https://learn.microsoft.com/en-us/dotnet/api/system.collections.frozen.frozendictionary-2)

* [Python's ChainMap for Java? @StackOverflow](https://stackoverflow.com/questions/30946588/pythons-chainmap-for-java)
