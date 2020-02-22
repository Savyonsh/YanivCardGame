using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Game2
{

    public enum Shapes { CLUBS, DIAMONDS, HEARTS, SPADES, JOKER }

    internal class Card : IComparable<Card>
    {
        public Vector2 Vector;
        public Rectangle Rectangle;

        public Card(Shapes shape, byte value, Texture2D texture)
        {
            Shape = shape;
            Value = value;
            Texture = texture;
            Picked = false;
            Rectangle = new Rectangle(79 * Value, 123 * (byte)Shape, 79, 123);
            Vector = new Vector2();
            CardState = CardState.NONE;
        }

        public void SetRectangle()
        {
            Rectangle.X = 79 * Value;
            Rectangle.Y = 123 * (byte)Shape;
        }

        public Card(Card other)
        {
            Shape = other.Shape;
            Value = other.Value;
            Texture = other.Texture;
            Picked = other.Picked;
            Vector = new Vector2(other.Vector.X, other.Vector.Y);
            Rectangle = new Rectangle(79 * Value, 123 * (byte)Shape, 79, 123);
        }

        public CardState CardState { get; set; }
        public Texture2D Texture { get; set; }

        public byte Value { get; set; }

        public Shapes Shape { get; set; }

        public bool Picked { get; set; }

        public int CompareTo(Card other)
        {
            return Value - other.Value;
        }
       
    }
}
