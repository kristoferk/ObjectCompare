# ObjectCompare

Shows the differences between the two objects.

## Installation
To install, run the following command in the Package Manager Console:
````shell

PM> Install-Package ObjectCompare.Linq

````

## Dokumentation
Apart from this README, you can find details and examples of using the SDK in the following places:  

- [Wiki](https://github.com/kristoferk/ObjectCompare/wiki)

## Usage
````csharp

var myObject = new MyObject { MyProperty = 3 };

var comparer = new ObjectComparer<MyObject>();
comparer.Config(p => {
    p.Field(f => f.MyProperty);
});

var track = comparer.Track(myObject);
myObject.MyProperty = 4;

var result = track.GetDiff();

Console.Write(string.Join("\n", result.Differences.Select(d => d.FormattedString)));

//Output: MyProperty: "3" => "4"

````
