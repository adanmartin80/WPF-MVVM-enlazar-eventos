    public class EventCommand : FrameworkElement
    {
        private Delegate _handler;

        public DelegateCommand<EventCommandArgs> Command
        {
            get { return (DelegateCommand<EventCommandArgs>)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(DelegateCommand<EventCommandArgs>), typeof(EventCommand), new PropertyMetadata(new PropertyChangedCallback(HandleCommandProperty)));

        public string Event
        {
            get { return (string)GetValue(EventProperty); }
            set { SetValue(EventProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Event.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EventProperty =
            DependencyProperty.Register("Event", typeof(string), typeof(EventCommand), new PropertyMetadata(new PropertyChangedCallback(HandleCommandProperty)));


        public Type RegisterElementType
        {
            get { return (Type)GetValue(RegisterElementTypeProperty); }
            set { SetValue(RegisterElementTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RegisterElementTypeProperty =
            DependencyProperty.Register("RegisterElementType", typeof(Type), typeof(EventCommand), new PropertyMetadata(new PropertyChangedCallback(HandleCommandProperty)));



        public DependencyObject RegisterElement
        {
            get { return (DependencyObject)GetValue(RegisterElementProperty); }
            set { SetValue(RegisterElementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RegisterElement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RegisterElementProperty =
            DependencyProperty.Register("RegisterElement", typeof(DependencyObject), typeof(EventCommand), new PropertyMetadata(new PropertyChangedCallback(HandleCommandProperty)));

        


        public object Parameter
        {
            get { return (object)GetValue(ParameterProperty); }
            set { SetValue(ParameterProperty, value); }
        }
        
        // Using a DependencyProperty as the backing store for Parameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParameterProperty =
            DependencyProperty.Register("Parameter", typeof(object), typeof(EventCommand), new PropertyMetadata(new PropertyChangedCallback(HandleCommandProperty)));

        

        private static void HandleCommandProperty(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var @this = d as EventCommand;
            if (@this.Event == null || (@this.RegisterElementType == null && @this.RegisterElement == null))
                return;

            DependencyObject elemento = @this.GetElement();

            if (elemento == null)
                return;

            var evento = elemento.GetType().GetEvent(@this.Event);
            if (evento == null)
                return;

            var handle = @this.CreateHandle(evento, @this.Handle);


            if (e.OldValue == null && e.NewValue != null)
            {
                evento.RemoveEventHandler(elemento, handle);
                evento.AddEventHandler(elemento, handle);
            }
            else if (e.OldValue != null && e.NewValue == null)
            {
                evento.RemoveEventHandler(elemento, handle);
            }
        }

        private void Handle(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
            {
                if (this.Command == null)
                    return;

                var eventArg = new EventCommandArgs
                {
                    Event = this.Event,
                    OriginControl = this.GetElement(),
                    Parameter = this.Parameter
                };

                if (this.Command.CanExecute(eventArg))
                    this.Command.Execute(eventArg);
            }));
        }

        private DependencyObject GetElement()
        {
            DependencyObject elemento = null;
            if (this.RegisterElementType != null && this.RegisterElement == null)
            {
                if (this.RegisterElementType == typeof(EventCommand))
                {
                    elemento = this;
                }
                else
                {
                    elemento = this.FindLogicalParent(this.RegisterElementType) as DependencyObject;
                }
            }
            else if (this.RegisterElement != null)
            {
                elemento = this.RegisterElement;
            }
            else
            {
                elemento = this;
            }

            return elemento;
        }

        private Delegate CreateHandle(EventInfo eventInfo, Action<object, EventArgs> action)
        {
            if (this._handler == null)
            {
                var parameters = eventInfo.EventHandlerType
                  .GetMethod("Invoke")
                  .GetParameters()
                  .Select(parameter => System.Linq.Expressions.Expression.Parameter(parameter.ParameterType))
                  .ToArray();

                var invoke = action.GetType().GetMethod("Invoke");

                this._handler = System.Linq.Expressions.Expression.Lambda(
                    eventInfo.EventHandlerType,
                    System.Linq.Expressions.Expression.Call(System.Linq.Expressions.Expression.Constant(action), invoke, parameters[0], parameters[1]),
                    parameters
                  )
                  .Compile();

            }
            return this._handler;
        }
    }

    public class EventCommandArgs : EventArgs
    {
        public DependencyObject OriginControl { get; set; }
        public string Event { get; set; }

        public object Parameter { get; set; }
    }
