using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace HistoryBuffer.Tests
{
    [TestFixture]
    internal class HistoryBufferTests
    {
        [Test]
        public void Can_remember_new_item()
        {
            //arrange
            var history = new HistoryBuffer<string>();

            string rememberedItem = null;
            history.NewItemRemembered += delegate(object sender, HistoryEventArgs<string> args)
            {
                args.TryGetCurrentItem(out rememberedItem);
            };
            
            //act
            history.RememberNew("new item");

            //assert
            rememberedItem.Should().Be("new item");
            history.CurrentItem.Should().Be("new item");
            history.Count.Should().Be(1);
        }
        

        [Test]
        public void Can_forget_current_item()
        {
            //arrange
            var history = new HistoryBuffer<string>();

            string forgottenItem = null;
            history.CurrentItemForgotten += delegate (object sender, HistoryEventArgs<string> args)
            {
                args.TryGetPreviousItem(out forgottenItem);
            };

            //act
            history.RememberNew("new item");
            history.ForgetCurrent();

            //assert
            forgottenItem.Should().Be("new item");

            Action getCurrentItem = () =>
            {
                var currentItem = history.CurrentItem;
            };
            getCurrentItem.ShouldThrow<HistoryIsEmptyException>();

            history.Count.Should().Be(0);
        }

        [Test]
        public void Can_undo()
        {
            //arrange
            var history = new HistoryBuffer<string>();

            string currentItem = null, previousItem = null;
            history.HistoryUndid += delegate (object sender, HistoryEventArgs<string> args)
            {
                args.TryGetCurrentItem(out currentItem);
                args.TryGetPreviousItem(out previousItem);
            };

            //act
            history.RememberNew("first item");
            history.RememberNew("second item");
            history.Undo();

            //assert
            currentItem.Should().Be("first item");
            previousItem.Should().Be("second item");

            history.CurrentItem.Should().Be("first item");
            history.Count.Should().Be(2);
        }

        [Test]
        public void Can_repeat()
        {
            //arrange
            var history = new HistoryBuffer<string>();

            string currentItem = null, previousItem = null;
            history.HistoryRepeated += delegate (object sender, HistoryEventArgs<string> args)
            {
                args.TryGetCurrentItem(out currentItem);
                args.TryGetPreviousItem(out previousItem);
            };

            //act
            history.RememberNew("first item");
            history.RememberNew("second item");
            history.Undo();
            history.Repeat();

            //assert
            currentItem.Should().Be("second item");
            previousItem.Should().Be("first item");

            history.CurrentItem.Should().Be("second item");
            history.Count.Should().Be(2);
        }

        [Test]
        public void Should_remove_obsolete_items_after_adding_new_one()
        {
            //arrange
            var history = new HistoryBuffer<string>();

            //act
            history.RememberNew("first item");
            history.RememberNew("second item");
            history.Undo();
            history.RememberNew("third item");

            //assert
            history.CurrentItem.Should().Be("third item");
            history.Count.Should().Be(2);
        }

        [Test]
        public void Can_overflow_limited_history()
        {
            //arrange
            var history = new HistoryBuffer<string>(maxSize: 2);

            string currentItem = null, previousItem = null;
            history.HistoryOverflowed += delegate (object sender, HistoryEventArgs<string> args)
            {
                args.TryGetCurrentItem(out currentItem);
                args.TryGetPreviousItem(out previousItem);
            };

            //act
            history.RememberNew("first item");
            history.RememberNew("second item");
            history.RememberNew("third item");

            //assert
            currentItem.Should().Be("third item");
            previousItem.Should().Be("second item");

            history.GetAll().ToArray().Should().BeEquivalentTo(new[]
            {
                "second item", "third item"
            });
        }
    }
}
