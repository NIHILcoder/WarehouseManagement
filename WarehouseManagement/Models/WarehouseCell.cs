using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using WarehouseManagement.Utils;

namespace WarehouseManagement.Models
{
    public class WarehouseCell
    {
        public int CellID { get; set; }
        public int ZoneID { get; set; }
        public string ZoneName { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public int UsedCapacity { get; set; }
        public int? ProductID { get; set; }
        public string ProductName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Получение списка всех ячеек
        public static List<WarehouseCell> GetAllCells()
        {
            List<WarehouseCell> cells = new List<WarehouseCell>();

            string query = @"
                SELECT c.*, z.Name AS ZoneName, p.Name AS ProductName
                FROM WarehouseCells c
                LEFT JOIN WarehouseZones z ON c.ZoneID = z.ZoneID
                LEFT JOIN Products p ON c.ProductID = p.ProductID
                ORDER BY z.Name, c.Location";

            try
            {
                var dataTable = DatabaseHelper.ExecuteQuery(query);

                foreach (DataRow row in dataTable.Rows)
                {
                    cells.Add(new WarehouseCell
                    {
                        CellID = Convert.ToInt32(row["CellID"]),
                        ZoneID = Convert.ToInt32(row["ZoneID"]),
                        ZoneName = row["ZoneName"].ToString(),
                        Location = row["Location"].ToString(),
                        Capacity = Convert.ToInt32(row["Capacity"]),
                        UsedCapacity = Convert.ToInt32(row["UsedCapacity"]),
                        ProductID = row["ProductID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["ProductID"]),
                        ProductName = row["ProductName"] == DBNull.Value ? null : row["ProductName"].ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"]),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                        UpdatedAt = Convert.ToDateTime(row["UpdatedAt"])
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении списка ячеек: " + ex.Message);
            }

            return cells;
        }

        // Получение ячеек по зоне
        public static List<WarehouseCell> GetCellsByZone(int zoneID)
        {
            List<WarehouseCell> cells = new List<WarehouseCell>();

            string query = @"
                SELECT c.*, z.Name AS ZoneName, p.Name AS ProductName
                FROM WarehouseCells c
                LEFT JOIN WarehouseZones z ON c.ZoneID = z.ZoneID
                LEFT JOIN Products p ON c.ProductID = p.ProductID
                WHERE c.ZoneID = @ZoneID
                ORDER BY c.Location";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@ZoneID", zoneID)
            };

            try
            {
                var dataTable = DatabaseHelper.ExecuteQuery(query, parameters);

                foreach (DataRow row in dataTable.Rows)
                {
                    cells.Add(new WarehouseCell
                    {
                        CellID = Convert.ToInt32(row["CellID"]),
                        ZoneID = Convert.ToInt32(row["ZoneID"]),
                        ZoneName = row["ZoneName"].ToString(),
                        Location = row["Location"].ToString(),
                        Capacity = Convert.ToInt32(row["Capacity"]),
                        UsedCapacity = Convert.ToInt32(row["UsedCapacity"]),
                        ProductID = row["ProductID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["ProductID"]),
                        ProductName = row["ProductName"] == DBNull.Value ? null : row["ProductName"].ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"]),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                        UpdatedAt = Convert.ToDateTime(row["UpdatedAt"])
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении ячеек зоны: " + ex.Message);
            }

            return cells;
        }

        // Получение ячейки по ID
        public static WarehouseCell GetCellByID(int cellID)
        {
            string query = @"
                SELECT c.*, z.Name AS ZoneName, p.Name AS ProductName
                FROM WarehouseCells c
                LEFT JOIN WarehouseZones z ON c.ZoneID = z.ZoneID
                LEFT JOIN Products p ON c.ProductID = p.ProductID
                WHERE c.CellID = @CellID";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@CellID", cellID)
            };

            try
            {
                var dataTable = DatabaseHelper.ExecuteQuery(query, parameters);

                if (dataTable.Rows.Count > 0)
                {
                    var row = dataTable.Rows[0];

                    return new WarehouseCell
                    {
                        CellID = Convert.ToInt32(row["CellID"]),
                        ZoneID = Convert.ToInt32(row["ZoneID"]),
                        ZoneName = row["ZoneName"].ToString(),
                        Location = row["Location"].ToString(),
                        Capacity = Convert.ToInt32(row["Capacity"]),
                        UsedCapacity = Convert.ToInt32(row["UsedCapacity"]),
                        ProductID = row["ProductID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["ProductID"]),
                        ProductName = row["ProductName"] == DBNull.Value ? null : row["ProductName"].ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"]),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                        UpdatedAt = Convert.ToDateTime(row["UpdatedAt"])
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении ячейки: " + ex.Message);
            }
        }

        // Поиск ячеек по названию товара
        public static List<WarehouseCell> FindCellsByProduct(string productName)
        {
            List<WarehouseCell> cells = new List<WarehouseCell>();

            string query = @"
                SELECT c.*, z.Name AS ZoneName, p.Name AS ProductName
                FROM WarehouseCells c
                LEFT JOIN WarehouseZones z ON c.ZoneID = z.ZoneID
                JOIN Products p ON c.ProductID = p.ProductID
                WHERE p.Name ILIKE @ProductName
                ORDER BY z.Name, c.Location";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@ProductName", $"%{productName}%")
            };

            try
            {
                var dataTable = DatabaseHelper.ExecuteQuery(query, parameters);

                foreach (DataRow row in dataTable.Rows)
                {
                    cells.Add(new WarehouseCell
                    {
                        CellID = Convert.ToInt32(row["CellID"]),
                        ZoneID = Convert.ToInt32(row["ZoneID"]),
                        ZoneName = row["ZoneName"].ToString(),
                        Location = row["Location"].ToString(),
                        Capacity = Convert.ToInt32(row["Capacity"]),
                        UsedCapacity = Convert.ToInt32(row["UsedCapacity"]),
                        ProductID = row["ProductID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["ProductID"]),
                        ProductName = row["ProductName"] == DBNull.Value ? null : row["ProductName"].ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"]),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                        UpdatedAt = Convert.ToDateTime(row["UpdatedAt"])
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при поиске ячеек: " + ex.Message);
            }

            return cells;
        }

        // Добавление новой ячейки
        public bool AddCell()
        {
            string query = @"
                INSERT INTO WarehouseCells (ZoneID, Location, Capacity, UsedCapacity, ProductID, IsActive)
                VALUES (@ZoneID, @Location, @Capacity, @UsedCapacity, @ProductID, @IsActive)
                RETURNING CellID";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@ZoneID", ZoneID),
                new NpgsqlParameter("@Location", Location),
                new NpgsqlParameter("@Capacity", Capacity),
                new NpgsqlParameter("@UsedCapacity", UsedCapacity),
                new NpgsqlParameter("@ProductID", (object)ProductID ?? DBNull.Value),
                new NpgsqlParameter("@IsActive", IsActive)
            };

            try
            {
                var result = DatabaseHelper.ExecuteScalar(query, parameters);
                CellID = Convert.ToInt32(result);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при добавлении ячейки: " + ex.Message);
            }
        }

        // Обновление ячейки
        public bool UpdateCell()
        {
            string query = @"
                UPDATE WarehouseCells
                SET ZoneID = @ZoneID,
                    Location = @Location,
                    Capacity = @Capacity,
                    UsedCapacity = @UsedCapacity,
                    ProductID = @ProductID,
                    IsActive = @IsActive,
                    UpdatedAt = CURRENT_TIMESTAMP
                WHERE CellID = @CellID";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@CellID", CellID),
                new NpgsqlParameter("@ZoneID", ZoneID),
                new NpgsqlParameter("@Location", Location),
                new NpgsqlParameter("@Capacity", Capacity),
                new NpgsqlParameter("@UsedCapacity", UsedCapacity),
                new NpgsqlParameter("@ProductID", (object)ProductID ?? DBNull.Value),
                new NpgsqlParameter("@IsActive", IsActive)
            };

            try
            {
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при обновлении ячейки: " + ex.Message);
            }
        }

        // Удаление ячейки
        public static bool DeleteCell(int cellID)
        {
            string query = "DELETE FROM WarehouseCells WHERE CellID = @CellID";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@CellID", cellID)
            };

            try
            {
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при удалении ячейки: " + ex.Message);
            }
        }

        // Освобождение ячейки (удаление товара из ячейки)
        public bool ClearCell()
        {
            string query = @"
                UPDATE WarehouseCells
                SET ProductID = NULL,
                    UsedCapacity = 0,
                    UpdatedAt = CURRENT_TIMESTAMP
                WHERE CellID = @CellID";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@CellID", CellID)
            };

            try
            {
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (rowsAffected > 0)
                {
                    ProductID = null;
                    ProductName = null;
                    UsedCapacity = 0;
                }
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при освобождении ячейки: " + ex.Message);
            }
        }

        // Получение всех зон склада
        public static List<WarehouseZone> GetAllZones()
        {
            List<WarehouseZone> zones = new List<WarehouseZone>();

            string query = "SELECT * FROM WarehouseZones ORDER BY Name";

            try
            {
                var dataTable = DatabaseHelper.ExecuteQuery(query);

                foreach (DataRow row in dataTable.Rows)
                {
                    zones.Add(new WarehouseZone
                    {
                        ZoneID = Convert.ToInt32(row["ZoneID"]),
                        Name = row["Name"].ToString(),
                        Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении списка зон: " + ex.Message);
            }

            return zones;
        }

        // Класс для представления зоны склада
        public class WarehouseZone
        {
            public int ZoneID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }
}