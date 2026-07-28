namespace Intent.Modules.Entities.BasicAuditing.Settings
{
    public static class BasicAuditingSettingsExtensions
    {
        // Whether an audit field is included is controlled by its "Include <Field>" switch, not by the
        // field name being blank - a blank text override is indistinguishable from "unset" once persisted,
        // so it can't reliably signal exclusion. Treat anything other than an explicit "false" as included,
        // matching the switch's own Default Value of "true".
        public static bool HasCreatedByField(this BasicAuditing settings) =>
            settings.GetSetting("16565573-5078-41c7-b083-e68b51154782")?.Value != "false";

        public static bool HasCreatedDateField(this BasicAuditing settings) =>
            settings.GetSetting("fb3a6f03-017f-499d-b44b-cb519b653ca6")?.Value != "false";

        public static bool HasUpdatedByField(this BasicAuditing settings) =>
            settings.GetSetting("5e86a4ca-df1d-4f63-a1db-c90a4fb70d24")?.Value != "false";

        public static bool HasUpdatedDateField(this BasicAuditing settings) =>
            settings.GetSetting("f936421d-7f37-4205-a98b-deea631a9291")?.Value != "false";

        public static bool HasAnyCreatedAuditField(this BasicAuditing settings) =>
            settings.HasCreatedByField() || settings.HasCreatedDateField();

        public static bool HasAnyUpdatedAuditField(this BasicAuditing settings) =>
            settings.HasUpdatedByField() || settings.HasUpdatedDateField();
    }
}
