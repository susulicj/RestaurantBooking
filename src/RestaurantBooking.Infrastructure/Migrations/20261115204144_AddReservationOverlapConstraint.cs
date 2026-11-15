using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationOverlapConstraint : Migration
    {
        /// <inheritdoc />
       protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS btree_gist;");

            migrationBuilder.Sql("""
                ALTER TABLE "Reservations"
                ADD CONSTRAINT "EX_Reservations_NoOverlap"
                EXCLUDE USING gist (
                    "DiningTableId" WITH =,
                    tstzrange("StartTimeUtc", "EndTimeUtc") WITH &&
                )
                WHERE ("Status" <> 'Cancelled');
                """);
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""ALTER TABLE "Reservations" DROP CONSTRAINT "EX_Reservations_NoOverlap";""");
        }
            }
}
