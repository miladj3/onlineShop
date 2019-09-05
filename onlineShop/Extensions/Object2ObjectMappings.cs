using Ganss.XSS;
using onlineShop.DTOs;
using onlineShop.Models;
using onlineShop.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace onlineShop.Extensions
{
    public static class Object2ObjectMappings
    {
        public static void ProductToProductViewModel(Product product, ProductViewModel viewModel)
        {
            viewModel.Id = product.Id;
            viewModel.IsActive = product.IsActive;
            viewModel.Name = product.Name;
            viewModel.ProducerCode = product.ProducerCode;
            viewModel.CatalogCode = product.CatalogCode;
            viewModel.SalePrice = product.SalePrice;
            viewModel.NumberInStock = product.NumberInStock;
            viewModel.AddedOn = product.AddedOn;
            viewModel.AddedBy = product.AddedBy;
            viewModel.LastModifiedOn = product.LastModifiedOn;
            viewModel.LastModifiedById = product.LastModifiedById;
            viewModel.LastModifiedByName = (product.LastModifiedBy == null) ? null : (product.LastModifiedBy.UserName);
            viewModel.SubcategoryId = product.SubcategoryId;
            viewModel.ProductDescriptionItems = product.ProductDescriptionItems;
            viewModel.Pictures = product.Pictures;
            viewModel.Comments = product.Comments;
            viewModel.ChangeHistory = product.ChangeHistory;
            viewModel.ExtendedDescriptionMarkup = WebUtility.HtmlDecode(product.ExtendedDescriptionMarkup);
        }

        public static void ProductViewModelToProduct(ProductViewModel viewModel, Product product)
        {
            product.IsActive = viewModel.IsActive;
            product.Name = viewModel.Name;
            product.ProducerCode = viewModel.ProducerCode;
            product.CatalogCode = viewModel.CatalogCode;
            product.SalePrice = viewModel.SalePrice;
            product.NumberInStock = viewModel.NumberInStock;
            product.Comments = viewModel.Comments;
            
            var sanitizer = new HtmlSanitizer();
            product.ExtendedDescriptionMarkup = WebUtility.HtmlEncode(sanitizer.Sanitize(viewModel.ExtendedDescriptionMarkup));
                       
            // copy description items and add missing ones
            foreach (var descItemVM in viewModel.ProductDescriptionItems)
            {
                var descItem = product.ProductDescriptionItems.FirstOrDefault(pdi => pdi.Id == descItemVM.Id && pdi.Id > 0);

                if (descItem != null)
                {
                    descItem.Value = descItemVM.Value;
                }
                else
                {
                    product.ProductDescriptionItems.Add(new ProductDescriptionItem
                    {
                        FieldId = descItemVM.FieldId,
                        Value = descItemVM.Value
                    });
                }
            }
        }

        public static void OrderToOrderDetailsDto(Order order, OrderDetailsDTO detailsDTO)
        {
            // set delivery method
            detailsDTO.DeliveryMethodType = order.DeliveryDetails.DeliveryMethodType;
            detailsDTO.PaymentMethod = order.PaymentMethod;

            // set common details
            detailsDTO.Firstname = order.DeliveryDetails.Firstname;
            detailsDTO.Lastname = order.DeliveryDetails.Lastname;
            detailsDTO.PhoneNumber = order.DeliveryDetails.PhoneNumber;
            detailsDTO.Email = order.DeliveryDetails.EmailAddress;
            detailsDTO.Comment = order.DeliveryDetails.Comment;

            switch (order.DeliveryDetails.DeliveryMethodType)
            {
                case DeliveryMethodType.Courier:

                    // set relevant delivery details properties
                    detailsDTO.Appartment = order.DeliveryDetails.CourierData.Appartment;
                    detailsDTO.Building = order.DeliveryDetails.CourierData.Building;
                    detailsDTO.City = order.DeliveryDetails.CourierData.City;
                    detailsDTO.Country = order.DeliveryDetails.CourierData.Country;
                    detailsDTO.PostalCode = order.DeliveryDetails.CourierData.PostalCode;
                    detailsDTO.Street = order.DeliveryDetails.CourierData.Street;

                    break;

                case DeliveryMethodType.Locker:

                    detailsDTO.ParcelLockerCode = order.DeliveryDetails.LockerData.LockerCode;

                    break;

                case DeliveryMethodType.Store:

                    detailsDTO.StoreCode = order.DeliveryDetails.StoreData.StoreCode;

                    break;

                default:
                    break;
            }
        }

        public static void OrderDetailsDtoToOrder(OrderDetailsDTO detailsDTO, Order order)
        {
            // set delivery method
            order.DeliveryDetails.DeliveryMethodType = detailsDTO.DeliveryMethodType;

            // set payment method
            order.PaymentMethod = detailsDTO.PaymentMethod;

            // set common details
            order.DeliveryDetails.Firstname = detailsDTO.Firstname;
            order.DeliveryDetails.Lastname = detailsDTO.Lastname;
            order.DeliveryDetails.PhoneNumber = detailsDTO.PhoneNumber;
            order.DeliveryDetails.EmailAddress = detailsDTO.Email;
            order.DeliveryDetails.Comment = detailsDTO.Comment;

            switch (detailsDTO.DeliveryMethodType)
            {
                case DeliveryMethodType.Courier:

                    // make sure container is not null (in case of delivery method change)
                    if (order.DeliveryDetails.CourierData == null)
                        order.DeliveryDetails.CourierData = new CourierData();

                    // set relevant delivery details properties
                    order.DeliveryDetails.CourierData.Appartment = detailsDTO.Appartment;
                    order.DeliveryDetails.CourierData.Building = detailsDTO.Building;
                    order.DeliveryDetails.CourierData.City = detailsDTO.City;
                    order.DeliveryDetails.CourierData.Country = detailsDTO.Country;
                    order.DeliveryDetails.CourierData.PostalCode = detailsDTO.PostalCode;
                    order.DeliveryDetails.CourierData.Street = detailsDTO.Street;

                    // set irrelevant data containers to null (in case of delivery method change)
                    order.DeliveryDetails.LockerData = null;
                    order.DeliveryDetails.StoreData = null;

                    break;

                case DeliveryMethodType.Locker:

                    if (order.DeliveryDetails.LockerData == null)
                        order.DeliveryDetails.LockerData = new LockerData();

                    order.DeliveryDetails.LockerData.LockerCode = detailsDTO.ParcelLockerCode;

                    order.DeliveryDetails.CourierData = null;
                    order.DeliveryDetails.StoreData = null;
                    break;

                case DeliveryMethodType.Store:

                    if (order.DeliveryDetails.StoreData == null)
                        order.DeliveryDetails.StoreData = new StoreData();

                    order.DeliveryDetails.StoreData.StoreCode = detailsDTO.StoreCode;

                    order.DeliveryDetails.CourierData = null;
                    order.DeliveryDetails.LockerData = null;
                    break;

                default:
                    break;
            }
        }

        public static void OrderItemListToOrderItemDtoList(List<OrderItem> orderItems, List<OrderItemDTO> orderItemDtos)
        {
            foreach (var item in orderItems)
            {
                orderItemDtos.Add(new OrderItemDTO
                {
                    Id = item.Id,
                    Product = new ProductDTO
                    {
                        Id = item.Product.Id,
                        Name = item.Product.Name,
                        Price = item.Product.SalePrice
                    },
                    Quantity = item.Quantity
                });
            }
        }

        public static void OrderItemsToOrderItemsDto(Order order, OrderDTO orderDto)
        {
            foreach (var orderItem in order.Items)
            {
                orderDto.Items.Add(new OrderItemDTO
                {
                    Id = orderItem.Id,
                    Product = new ProductDTO
                    {
                        Id = orderItem.ProductId,
                        Price = orderItem.PurchasePrice,
                        Name = orderItem.Product.Name
                    },
                    Quantity = orderItem.Quantity
                });
            }
        }

        public static void OrderMainDataToOrderDto(Order order, OrderDTO orderDto)
        {
            orderDto.Id = order.Id;
            orderDto.CreatedOn = order.CreatedOn;
            orderDto.LastModifiedOn = order.LastModifiedOn;
            orderDto.CompletedOn = order.CompletedOn;
            orderDto.LastModifiedById = order.LastModifiedById;
            orderDto.LastModifiedByName = (order.LastModifiedBy == null) ? null : (order.LastModifiedBy.UserName);
            orderDto.Status = order.Status;
            orderDto.ChangeHistory = order.ChangeHistory;
            orderDto.DeliveryFee = order.DeliveryFee;
        }
    }
}