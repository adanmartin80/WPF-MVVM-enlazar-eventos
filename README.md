# WPF-MVVM-enlazar-eventos
Define una clase que permite enlazar eventos a los comandos del ViewModel

>######Definición de la clase
***
Esta linea:
```C#
_Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render..._
```
Permite que el delegado que ejecuta el dispatcher se ejecute en prioridad de renderización, por lo que todos los binding asociados al 
elemento se habrán completado.
</br>El metodo `CreateHandle()` es capaz de construir el delagado necesario para subscribirlo al evento que se haya configurado.
</br>El metodo `GetElement()` busca el tipo del padre indicado en el arbol logico. Usa la siguiente extensión:
```C#
        public static object FindLogicalParent(this DependencyObject obj, Type type, bool IsAssignableFrom = false)
        {
            try
            {
                if (obj == null || ((obj as Visual) == null && (obj as UIElement3D) == null))
                    return null;
                DependencyObject parent = LogicalTreeHelper.GetParent(obj);
                if (parent != null && (parent.GetType() == type || (IsAssignableFrom && type.IsAssignableFrom(parent.GetType()))))
                    return parent;
                else
                {
                    var parentOfParent = FindLogicalParent(parent, type);
                    if (parentOfParent != null)
                        return parentOfParent;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }
```
</br>El metodo `HandleCommandProperty()` determina cuando se esta creando el objeto en XAML y cuando se esta destruyendo, sabiendo ésto, 
realiza la logica necesaria para subscribir el manejador al evento indicado pasandole al comando los parametros definidos.
</br>El comando a configurar usa el tipo `EventCommandArgs` por lo que su definición ha de ser algo parecido a:
```C#
public DelegateCommand<EventCommandArgs> ComandoInicio { get; set; }
```

Su uso en el XAML, dentro de la ```MainWindow``` para subscribir el comando al evento ```Loaded``` sera algo parecido a:
```xaml
<accion:EventCommand Command="{Binding ComandoInicio}" Event="Loaded" RegisterElementType="{x:Type vistas:MainWindow}" Parameter="{Binding ElementName=ElementoButton}" />
```
