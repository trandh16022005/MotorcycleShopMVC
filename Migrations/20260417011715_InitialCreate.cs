using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotorcycleShopMVC.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "brand",
                columns: table => new
                {
                    brand_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    brand_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__brand__5E5A8E27D6FBE891", x => x.brand_id);
                });

            migrationBuilder.CreateTable(
                name: "MotorcyclePart",
                columns: table => new
                {
                    MotorcycleId = table.Column<int>(type: "int", nullable: false),
                    PartId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotorcyclePart", x => new { x.MotorcycleId, x.PartId });
                });

            migrationBuilder.CreateTable(
                name: "part_category",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__part_cat__D54EE9B4BB9F450B", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "promotion",
                columns: table => new
                {
                    promotion_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    discount_type = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    discount_value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    min_order_value = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    start_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__promotio__2CB9556B251A8A0D", x => x.promotion_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    full_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    phone_number = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    role = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValue: "customer"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    payment_method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__users__3213E83F3F46CAE1", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_type",
                columns: table => new
                {
                    type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__vehicle___2C000598AB825BDC", x => x.type_id);
                });

            migrationBuilder.CreateTable(
                name: "part",
                columns: table => new
                {
                    part_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    part_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    category_id = table.Column<int>(type: "int", nullable: true),
                    brand_id = table.Column<int>(type: "int", nullable: true),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    stock_quantity = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    image_path = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    warranty_months = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__part__A0E3FAB8AE85F9B9", x => x.part_id);
                    table.ForeignKey(
                        name: "FK__part__brand_id__66603565",
                        column: x => x.brand_id,
                        principalTable: "brand",
                        principalColumn: "brand_id");
                    table.ForeignKey(
                        name: "FK__part__category_i__656C112C",
                        column: x => x.category_id,
                        principalTable: "part_category",
                        principalColumn: "category_id");
                });

            migrationBuilder.CreateTable(
                name: "cart",
                columns: table => new
                {
                    cart_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cart__2EF52A2736B8EABB", x => x.cart_id);
                    table.ForeignKey(
                        name: "FK__cart__user_id__6FE99F9F",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notification",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    content = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    reference_id = table.Column<int>(type: "int", nullable: true),
                    reference_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    is_read = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__notifica__E059842F23D1F62F", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK__notificat__user___22751F6C",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    promotion_id = table.Column<int>(type: "int", nullable: true),
                    order_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    total_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    shipping_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    payment_status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Pending"),
                    order_status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Processing"),
                    note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__order__4659622989CB449D", x => x.order_id);
                    table.ForeignKey(
                        name: "FK__order__user_id__00200768",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "motorcycle",
                columns: table => new
                {
                    motorcycle_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    model_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    brand_id = table.Column<int>(type: "int", nullable: false),
                    type_id = table.Column<int>(type: "int", nullable: true),
                    engine_capacity = table.Column<int>(type: "int", nullable: true),
                    year_from = table.Column<int>(type: "int", nullable: true),
                    year_to = table.Column<int>(type: "int", nullable: true),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    warranty_policy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    image_path = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    stock_qty = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__motorcyc__7340036B1F7C9B32", x => x.motorcycle_id);
                    table.ForeignKey(
                        name: "FK__motorcycl__brand__5AEE82B9",
                        column: x => x.brand_id,
                        principalTable: "brand",
                        principalColumn: "brand_id");
                    table.ForeignKey(
                        name: "FK__motorcycl__type___5BE2A6F2",
                        column: x => x.type_id,
                        principalTable: "vehicle_type",
                        principalColumn: "type_id");
                });

            migrationBuilder.CreateTable(
                name: "cart_item",
                columns: table => new
                {
                    cart_item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cart_id = table.Column<int>(type: "int", nullable: false),
                    motorcycle_id = table.Column<int>(type: "int", nullable: true),
                    part_id = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cart_ite__5D9A6C6ED6140EC7", x => x.cart_item_id);
                    table.ForeignKey(
                        name: "FK__cart_item__cart___73BA3083",
                        column: x => x.cart_id,
                        principalTable: "cart",
                        principalColumn: "cart_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__cart_item__motor__74AE54BC",
                        column: x => x.motorcycle_id,
                        principalTable: "motorcycle",
                        principalColumn: "motorcycle_id");
                    table.ForeignKey(
                        name: "FK__cart_item__part___75A278F5",
                        column: x => x.part_id,
                        principalTable: "part",
                        principalColumn: "part_id");
                });

            migrationBuilder.CreateTable(
                name: "order_item",
                columns: table => new
                {
                    order_item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    motorcycle_id = table.Column<int>(type: "int", nullable: true),
                    part_id = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__order_it__3764B6BC2A1FACB9", x => x.order_item_id);
                    table.ForeignKey(
                        name: "FK__order_ite__motor__03F0984C",
                        column: x => x.motorcycle_id,
                        principalTable: "motorcycle",
                        principalColumn: "motorcycle_id");
                    table.ForeignKey(
                        name: "FK__order_ite__order__02FC7413",
                        column: x => x.order_id,
                        principalTable: "order",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__order_ite__part___04E4BC85",
                        column: x => x.part_id,
                        principalTable: "part",
                        principalColumn: "part_id");
                });

            migrationBuilder.CreateTable(
                name: "part_motorcycle",
                columns: table => new
                {
                    part_id = table.Column<int>(type: "int", nullable: false),
                    motorcycle_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__part_mot__07D7FA8E3F4FEAC2", x => new { x.part_id, x.motorcycle_id });
                    table.ForeignKey(
                        name: "FK__part_moto__motor__6A30C649",
                        column: x => x.motorcycle_id,
                        principalTable: "motorcycle",
                        principalColumn: "motorcycle_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__part_moto__part___693CA210",
                        column: x => x.part_id,
                        principalTable: "part",
                        principalColumn: "part_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "review",
                columns: table => new
                {
                    review_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    motorcycle_id = table.Column<int>(type: "int", nullable: true),
                    part_id = table.Column<int>(type: "int", nullable: true),
                    rating = table.Column<int>(type: "int", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__review__60883D90AA4CCE2F", x => x.review_id);
                    table.ForeignKey(
                        name: "FK__review__motorcyc__0C85DE4D",
                        column: x => x.motorcycle_id,
                        principalTable: "motorcycle",
                        principalColumn: "motorcycle_id");
                    table.ForeignKey(
                        name: "FK__review__order_id__0B91BA14",
                        column: x => x.order_id,
                        principalTable: "order",
                        principalColumn: "order_id");
                    table.ForeignKey(
                        name: "FK__review__part_id__0D7A0286",
                        column: x => x.part_id,
                        principalTable: "part",
                        principalColumn: "part_id");
                    table.ForeignKey(
                        name: "FK__review__user_id__0A9D95DB",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "wishlist",
                columns: table => new
                {
                    wishlist_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    motorcycle_id = table.Column<int>(type: "int", nullable: true),
                    part_id = table.Column<int>(type: "int", nullable: true),
                    added_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__wishlist__6151514E992593C9", x => x.wishlist_id);
                    table.ForeignKey(
                        name: "FK__wishlist__motorc__1BC821DD",
                        column: x => x.motorcycle_id,
                        principalTable: "motorcycle",
                        principalColumn: "motorcycle_id");
                    table.ForeignKey(
                        name: "FK__wishlist__part_i__1CBC4616",
                        column: x => x.part_id,
                        principalTable: "part",
                        principalColumn: "part_id");
                    table.ForeignKey(
                        name: "FK__wishlist__user_i__1AD3FDA4",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "review_image",
                columns: table => new
                {
                    image_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    review_id = table.Column<int>(type: "int", nullable: false),
                    image_path = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__review_i__DC9AC955651A6002", x => x.image_id);
                    table.ForeignKey(
                        name: "FK__review_im__revie__114A936A",
                        column: x => x.review_id,
                        principalTable: "review",
                        principalColumn: "review_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UQ__brand__0C0C3B58AC12B969",
                table: "brand",
                column: "brand_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_cart_user",
                table: "cart",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ__cart__B9BE370EEF16442A",
                table: "cart",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cart_item_cart_id",
                table: "cart_item",
                column: "cart_id");

            migrationBuilder.CreateIndex(
                name: "IX_cart_item_motorcycle_id",
                table: "cart_item",
                column: "motorcycle_id");

            migrationBuilder.CreateIndex(
                name: "IX_cart_item_part_id",
                table: "cart_item",
                column: "part_id");

            migrationBuilder.CreateIndex(
                name: "IX_motorcycle_brand_id",
                table: "motorcycle",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_motorcycle_type_id",
                table: "motorcycle",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "IX_notification_user_id",
                table: "notification",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_order_user",
                table: "order",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_orderitem_order",
                table: "order_item",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_item_motorcycle_id",
                table: "order_item",
                column: "motorcycle_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_item_part_id",
                table: "order_item",
                column: "part_id");

            migrationBuilder.CreateIndex(
                name: "IX_part_brand_id",
                table: "part",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_part_category_id",
                table: "part",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "UQ__part_cat__5189E2554497F2DD",
                table: "part_category",
                column: "category_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_part_motorcycle_motorcycle_id",
                table: "part_motorcycle",
                column: "motorcycle_id");

            migrationBuilder.CreateIndex(
                name: "UQ__promotio__357D4CF9CDF8E738",
                table: "promotion",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_review_motorcycle_id",
                table: "review",
                column: "motorcycle_id");

            migrationBuilder.CreateIndex(
                name: "IX_review_order_id",
                table: "review",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_review_part_id",
                table: "review",
                column: "part_id");

            migrationBuilder.CreateIndex(
                name: "IX_review_user_id",
                table: "review",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_review_image_review_id",
                table: "review_image",
                column: "review_id");

            migrationBuilder.CreateIndex(
                name: "UQ__users__A1936A6BBF9DED3A",
                table: "users",
                column: "phone_number",
                unique: true,
                filter: "[phone_number] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__users__AB6E6164759D06FA",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__vehicle___543C4FD973271E93",
                table: "vehicle_type",
                column: "type_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_wishlist_motorcycle_id",
                table: "wishlist",
                column: "motorcycle_id");

            migrationBuilder.CreateIndex(
                name: "IX_wishlist_part_id",
                table: "wishlist",
                column: "part_id");

            migrationBuilder.CreateIndex(
                name: "IX_wishlist_user_id",
                table: "wishlist",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cart_item");

            migrationBuilder.DropTable(
                name: "MotorcyclePart");

            migrationBuilder.DropTable(
                name: "notification");

            migrationBuilder.DropTable(
                name: "order_item");

            migrationBuilder.DropTable(
                name: "part_motorcycle");

            migrationBuilder.DropTable(
                name: "promotion");

            migrationBuilder.DropTable(
                name: "review_image");

            migrationBuilder.DropTable(
                name: "wishlist");

            migrationBuilder.DropTable(
                name: "cart");

            migrationBuilder.DropTable(
                name: "review");

            migrationBuilder.DropTable(
                name: "motorcycle");

            migrationBuilder.DropTable(
                name: "order");

            migrationBuilder.DropTable(
                name: "part");

            migrationBuilder.DropTable(
                name: "vehicle_type");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "brand");

            migrationBuilder.DropTable(
                name: "part_category");
        }
    }
}
