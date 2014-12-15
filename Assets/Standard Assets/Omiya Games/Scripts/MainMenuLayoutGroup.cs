using UnityEngine;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    public class MainMenuLayoutGroup : GridLayoutGroup
    {
        public enum MainMenuConstraint
        {
            FixedColumnCount = 0,
            FixedRowCount
        }

        [SerializeField]
        protected MainMenuConstraint m_menuConstraint = MainMenuConstraint.FixedColumnCount;
        public MainMenuConstraint mainMenuConstraint
        {
            get
            {
                return m_menuConstraint;
            }
            set
            {
                SetProperty(ref m_menuConstraint, value);
            }
        }

        Vector2 tempCellSize = Vector2.one;

        protected MainMenuLayoutGroup()
        { }

        public override void CalculateLayoutInputHorizontal()
        {
            constraint = ConvertToConstraint(mainMenuConstraint);
            cellSize = CalculateCellSize(mainMenuConstraint, constraintCount);
            base.CalculateLayoutInputHorizontal();
        }

        public override void CalculateLayoutInputVertical()
        {
            constraint = ConvertToConstraint(mainMenuConstraint);
            cellSize = CalculateCellSize(mainMenuConstraint, constraintCount);
            base.CalculateLayoutInputVertical();
        }

        public override void SetLayoutHorizontal()
        {
            constraint = ConvertToConstraint(mainMenuConstraint);
            cellSize = CalculateCellSize(mainMenuConstraint, constraintCount);
            base.SetLayoutHorizontal();
        }

        public override void SetLayoutVertical()
        {
            constraint = ConvertToConstraint(mainMenuConstraint);
            cellSize = CalculateCellSize(mainMenuConstraint, constraintCount);
            base.SetLayoutVertical();
        }

        Constraint ConvertToConstraint(MainMenuConstraint menuConstraint)
        {
            Constraint returnEnum = Constraint.FixedColumnCount;
            if(menuConstraint == MainMenuConstraint.FixedRowCount)
            {
                returnEnum = Constraint.FixedRowCount;
            }
            return returnEnum;
        }

        Vector2 CalculateCellSize(MainMenuConstraint menuConstraint, int constraintQuantity)
        {
            // Calculate the number of columns and rows (assuming menuConstraint is set to default, i.e. FixedColumnCount)
            int numColumns = Mathf.Max(constraintQuantity, 1);
            int numRows = Mathf.Max((rectChildren.Count / numColumns), 1);
            if ((rectChildren.Count % numColumns) > 0)
            {
                ++numRows;
            }

            // Check if menuConstraint is actually set to default (i.e. FixedColumnCount)
            if(menuConstraint == MainMenuConstraint.FixedRowCount)
            {
                // If not, swap the values
                int swap = numColumns;
                numColumns = numRows;
                numRows = swap;
            }

            // Calculate how much space all the buttons cumulatively takes
            tempCellSize = rectTransform.rect.size;

            // Take out all the padding
            tempCellSize.x -= padding.left;
            tempCellSize.x -= padding.right;
            tempCellSize.y -= padding.top;
            tempCellSize.y -= padding.bottom;

            // Take out the button spacing
            tempCellSize.x -= (spacing.x * (numColumns - 1));
            tempCellSize.y -= (spacing.y * (numRows - 1));

            // Divide the cumulative space by the number of columns and rows
            tempCellSize.x /= numColumns;
            tempCellSize.y /= numRows;

            // Return the cell size
            return tempCellSize;
        }
    }
}