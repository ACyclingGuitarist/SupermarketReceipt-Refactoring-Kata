using System.Collections.Generic;
using Xunit;

namespace SupermarketReceipt.Test
{
    public class SupermarketXUnitTest
    {
        private readonly SupermarketCatalog _catalog;
        private readonly Teller _teller;
        private readonly Product _toothBrush;
        private readonly Product _apples;

        public SupermarketXUnitTest()
        {
            _toothBrush = new Product("toothbrush", ProductUnit.Each);
            _apples = new Product("apples", ProductUnit.Kilo);

            _catalog = new FakeCatalog();

            _teller = new Teller(_catalog);

            _catalog.AddProduct(_toothBrush, 0.99);
            _catalog.AddProduct(_apples, 1.99);
        }

        [Fact]
        public void NoItemsInCart()
        {
            var cart = new ShoppingCart();

            var receipt = _teller.ChecksOutArticlesFrom(cart);

            Assert.NotNull(receipt);
            Assert.Equal(0.0, receipt.GetTotalPrice());
            Assert.Empty(receipt.GetItems());
        }

        [Fact]
        public void SingleItemInCart()
        {
            var cart = new ShoppingCart();

            cart.AddItem(_toothBrush);

            var receipt = _teller.ChecksOutArticlesFrom(cart);

            Assert.NotNull(receipt);
            Assert.Empty(receipt.GetDiscounts());
            Assert.Single(receipt.GetItems());
            Assert.Equal(0.99, receipt.GetTotalPrice());
        }

        [Fact]
        public void MultipleItemsInCart() 
        {
            var cart = new ShoppingCart();

            cart.AddItem(_toothBrush);
            cart.AddItem(_toothBrush);

            var receipt = _teller.ChecksOutArticlesFrom(cart);

            Assert.NotNull(receipt);
            Assert.Empty(receipt.GetDiscounts());
            Assert.Equal(2, receipt.GetItems().Count);
            Assert.Equal(0.99 * 2, receipt.GetTotalPrice());
        }

        [Fact]
        public void WeightedItemInCart()
        {
            var cart = new ShoppingCart();

            cart.AddItem(_apples, 5);       

            var receipt = _teller.ChecksOutArticlesFrom(cart);

            Assert.NotNull(receipt);
            Assert.Empty(receipt.GetDiscounts());
            Assert.Single(receipt.GetItems());
            Assert.Equal(1.99 * 5, receipt.GetTotalPrice());
        }

        [Fact]
        public void TenPercentDiscount()
        {
            // ARRANGE
            var cart = new ShoppingCart();
            cart.AddItem(_apples, 2.5);

            var teller = new Teller(_catalog);
            teller.AddSpecialOffer(SpecialOfferType.TenPercentDiscount, _apples, 10.0);

            // ACT
            var receipt = teller.ChecksOutArticlesFrom(cart);

            // ASSERT
            Assert.Equal(4.975 - (4.975 * 0.10), receipt.GetTotalPrice());
            Assert.Single(receipt.GetDiscounts());
            Assert.Single(receipt.GetItems());
            var receiptItem = receipt.GetItems()[0];
            Assert.Equal(_apples, receiptItem.Product);
            Assert.Equal(1.99, receiptItem.Price);
            Assert.Equal(2.5 * 1.99, receiptItem.TotalPrice);
            Assert.Equal(2.5, receiptItem.Quantity);
        }
    }
}