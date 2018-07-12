using System;
using System.Collections.Generic;
using System.Linq;
using Orange.Library.Managers;
using Orange.Library.Messages;
using Standard.Internet.XML;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Orange.Library.Managers.MessageManager;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
   public class Graph : Value, IMessageHandler
   {
      const string LOCATION = "Graph";

      ValueGraph graph;

      public Graph(string name, Value value)
      {
         graph = new ValueGraph(name);
         var valueIsGraph = value.Type == ValueType.Graph;
         if (value.IsArray && !valueIsGraph)
         {
            var array = (Array)value.SourceArray;
            if (array.Length > 0 && array.All(i => i.Value.Type == ValueType.Graph))
               foreach (var valueGraph in array.Select(item => item.Value).OfType<Graph>().Select(child => child.graph))
                  graph[valueGraph.Name] = valueGraph;
            else
               graph.Value = array;
         }
         else if (name.IsNotEmpty() && valueIsGraph && ((Graph)value).graph.Name.IsEmpty())
         {
            var childGraph = (Graph)value;
            foreach (var item in childGraph.graph.Children)
               graph[item.Key] = item.Value;
         }
         else
            graph.Value = value;
      }

      public Graph(ValueGraph graph) => this.graph = graph;

      public override int Compare(Value value)
      {
         if (value is Graph otherValue)
         {
            var otherGraph = otherValue.graph;
            var compare = string.Compare(graph.Name, otherGraph.Name, StringComparison.Ordinal);
            return compare != 0 ? compare : graph.Value.Compare(otherGraph.Value);
         }

         return string.Compare(ToString(), value.ToString(), StringComparison.Ordinal);
      }

      public override string Text
      {
         get => "";
         set { }
      }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Graph;

      public override bool IsTrue => false;

      public override Value Clone() => new Graph(graph);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "name", v => ((Graph)v).Name());
         manager.RegisterProperty(this, "value", v => ((Graph)v).GetValue(), v => ((Graph)v).SetValue());
         manager.RegisterMessage(this, "children", v => ((Graph)v).Children());
         manager.RegisterMessage(this, "remove", v => ((Graph)v).Remove());
         manager.RegisterMessage(this, "xml", v => ((Graph)v).XML());
         manager.RegisterMessage(this, "if", v => ((Graph)v).If());
         manager.RegisterMessage(this, "arr", v => ((Graph)v).Array());
         manager.RegisterMessage(this, "new-message", v => ((Graph)v).NewMessage());
         manager.RegisterMessage(this, "map", v => ((Graph)v).Map());
         manager.RegisterMessage(this, "for", v => ((Graph)v).For());
      }

      public Value Name() => graph.Name;

      public Value Value() => new ValueAttributeVariable("value", this);

      public Value GetValue() => graph.Value;

      public Value SetValue()
      {
         graph.Value = Arguments[0];
         return this;
      }

      public Value Children()
      {
         var array = new Array();
         foreach (var item in graph.Children)
            array[item.Key] = new Graph(item.Value);

         return array;
      }

      public Value Send(Value value, string messageName, Arguments arguments, out bool handled)
      {
         handled = true;
         if (DefaultRespondsTo(messageName) && !MessagingState.RespondsToRegisteredMessage(value, messageName))
            return DefaultSendMessage(value, messageName, arguments, out handled);

         var childGraph = graph[messageName];
         if (childGraph == null)
         {
            childGraph = new ValueGraph(messageName);
            var variable = new GraphVariable(messageName, childGraph, graph);
            return variable;
         }

         if (childGraph.Value is Lambda closure)
            return dispatchClosure(closure, arguments);

         return childGraph.Children.Count > 0 ? (Value)new Graph(childGraph) : new GraphVariable(messageName, childGraph);
      }

      public bool RespondsTo(string messageName) => graph.Children.Select(i => i.Key).Any(k => k == messageName);

      Value dispatchClosure(Lambda lambda, Arguments arguments)
      {
         lambda.Block.AutoRegister = false;
         State.RegisterBlock(lambda.Block);
         Regions.SetLocal("self", this);
         var result = lambda.Evaluate(arguments, this);
         result = State.UseReturnValue(result);
         State.UnregisterBlock();
         return result;
      }

      public override string ToString() => graph.ToString();

      public override void AssignTo(Variable variable)
      {
         if (graph.Name == VAR_AUTOASSIGN)
            graph = graph.Duplicate(variable.Name);
         base.AssignTo(variable);
      }

      public Value Remove()
      {
         graph.Remove(Arguments[0].Text);
         return this;
      }

      public override Value AlternateValue(string message) => getValue(graph);

      static Value getValue(ValueGraph graph)
      {
         if (graph.Children.Count == 0)
            return graph.Value;

         var array = new Array();
         foreach (var item in graph.Children)
         {
            var value = getValue(item.Value);
            array[item.Value.Name] = value;
         }

         return array;
      }

      public Value XML()
      {
         var builder = new XMLBuilder(graph.Name);
         renderXML(graph, builder.Root.Value);
         return builder.ToString();
      }

      static void renderXML(ValueGraph graph, Element element)
      {
         if (graph.Children.Count > 0)
            foreach (var item in graph.Children)
            {
               var name = item.Key;
               name = name.Substitute("'-' /d+ $", "");
               name = name.Substitute("'_' /(/d+) $", "-$1");
               if (name.StartsWith("$"))
                  element.Attributes.Add(name.Skip(1), item.Value.Value.Text);
               else if (name == "text")
                  element.Text = item.Value.Value.Text;
               else
               {
                  var childElement = new Element { Name = name };
                  element.Children.Add(childElement);
                  renderXML(item.Value, childElement);
               }
            }
         else
            element.Text = graph.Value.Text;
      }

      public Value If()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return new Graph(graph.Name, "");

            assistant.IteratorParameter();
            if (graph.Children.Count == 0)
            {
               assistant.SetIteratorParameter(this);
               var value = block.IsTrue ? graph.Value : "";
               return new Graph(graph.Name, value);
            }

            var list = new List<Graph>();
            foreach (var item in graph.Children)
               addToList(assistant, block, item.Value, list);

            return new Graph(graph.Name, new Array(list));
         }
      }

      void addToList(ParameterAssistant assistant, Block block, Value value, List<Graph> list)
      {
         var graphValue = value as Graph;
         RejectNull(graphValue, LOCATION, "Item must be a Graph");
         addToList(assistant, block, graphValue.graph, list);
      }

      static void addToList(ParameterAssistant assistant, Block block, ValueGraph graph, List<Graph> list)
      {
         var value = new Graph(graph);
         assistant.SetIteratorParameter(value);
         if (block.IsTrue)
            list.Add(value);
         foreach (var item in graph.Children)
            addToList(assistant, block, item.Value, list);
      }

      public Value Array() => getValue(graph);

      public Value NewMessage()
      {
         var value = Arguments[0];
         if (value is Graph newGraph)
         {
            var innerGraph = newGraph.graph;
            graph[innerGraph.Name] = innerGraph;
            return this;
         }

         var message = value.Text;
         var valueGraph = new ValueGraph(message);
         graph[message] = valueGraph;
         return new GraphVariable(message, valueGraph);
      }

      public Value Map()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            var array = new Array();
            assistant.ArrayParameters();
            var index = 0;
            foreach (var item in graph.Children)
            {
               assistant.SetParameterValues(new Graph(item.Value), item.Key, index++);
               var value = block.Evaluate();
               var signal = ParameterAssistant.Signal();
               if (signal == ParameterAssistant.SignalType.Breaking)
                  break;

               switch (signal)
               {
                  case ParameterAssistant.SignalType.Continuing:
                     continue;
                  case ParameterAssistant.SignalType.ReturningNull:
                     return null;
               }

               array[item.Key] = value;
            }

            return array;
         }
      }

      public Value For()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            assistant.ArrayParameters();
            var index = 0;
            foreach (var item in graph.Children)
            {
               assistant.SetParameterValues(new Graph(item.Value), item.Key, index++);
               block.Evaluate();
               var signal = ParameterAssistant.Signal();
               if (signal == ParameterAssistant.SignalType.Breaking)
                  break;

               switch (signal)
               {
                  case ParameterAssistant.SignalType.Continuing:
                     continue;
                  case ParameterAssistant.SignalType.ReturningNull:
                     return null;
               }
            }

            return this;
         }
      }
   }
}