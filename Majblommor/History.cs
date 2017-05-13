using Majblommor.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Majblommor
{
    public static class History
    {
        private static List<Step> steps;
        private static int counter;
        private static bool canAdd = true;

        public static bool CanUndo { get { return counter > -1; } }
        public static bool CanRedo { get { return counter + 1 < steps.Count; } }

        static History()
        {
            steps = new List<Step>();
            counter = -1;
        }

        public static void ClearHistory()
        {
            steps = new List<Step>();
            counter = -1;
        }

        public static void Undo()
        {
            if(CanUndo)
            {
                canAdd = false;
                steps[counter].Undo();
                counter--;
                canAdd = true;
            }
        }

        public static void Redo()
        {
            if(CanRedo)
            {
                canAdd = false;
                counter++;
                steps[counter].Redo();
                canAdd = true;
            }
        }

        public static void NewStep()
        {
            counter++;
            if (counter < steps.Count)
            {
                steps.RemoveRange(counter, steps.Count - counter);
            }
            steps.Add(new Step());
        }

        public static void AddToHistory(ICommand command)
        {
            if (counter >= 0 && canAdd)
            {
                steps[counter].Commands.Add(command);
            }
        }
    }
}
