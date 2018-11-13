
        /// <summary>
        /// Delete all commnets for specific application
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="appName"></param>
        public static void DeleteCommnetsForApp(SqlConnection connection, string appName)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"DeleteAppLog";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                SqlParameter p1 =
                    new SqlParameter("appName", System.Data.SqlDbType.NVarChar, 100)
                    {
                        Value = appName
                    };

                command.Parameters.Add(p1);
                command.ExecuteNonQuery();

            }
        }
