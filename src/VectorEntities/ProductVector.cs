using DataEntities;
using System.ComponentModel.DataAnnotations;

namespace VectorEntities
{
    public class ProductVector : Product
    {
        [Key]
        public override int Id { get => base.Id; set => base.Id = value; }

        public override string? Name { get => base.Name; set => base.Name = value; }

        public override string? Description { get => base.Description; set => base.Description = value; }

        public override decimal Price { get => base.Price; set => base.Price = value; }

        public ReadOnlyMemory<float> Vector { get; set; }
    }
}
