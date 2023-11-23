namespace G4ME.SourceBuilder.Syntax;

public class PropertyDeclarationCollection
{
    private readonly List<PropertyDeclarationSyntax> _properties = [ ];
    private int _currentIndex = -1;

    public PropertyDeclarationSyntax CurrentProperty
    {
        get
        {
            if (!_properties.Any())
            {
                throw new InvalidOperationException("No current property. Ensure a property is added before accessing.");
            }

            return _properties[_currentIndex];
        }
    }

    public void AddProperty(PropertyDeclarationSyntax property)
    {
        _properties.Add(property);
        _currentIndex++;
    }

    public void UpdateCurrentProperty(PropertyDeclarationSyntax updatedProperty)
    {
        if (_currentIndex == -1) throw new InvalidOperationException("No property to update.");
        
        _properties[_currentIndex] = updatedProperty;
    }

    public PropertyDeclarationSyntax[] Build() => _properties.ToArray();
}
