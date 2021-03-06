﻿namespace Microsoft.Win32
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    /// <summary>
    /// Represents a folder browser dialog.
    /// </summary>
    public class FolderBrowserDialog : CommonDialog
    {
        private string title = string.Empty;
        private string selectedPath = string.Empty;
        private Environment.SpecialFolder rootFolder = Environment.SpecialFolder.Desktop;

        /// <summary>
        /// Gets or sets the descriptive text displayed in the dialog box title.
        /// </summary>
        /// <value>The title to display. The default is an empty string ("").</value>
        [DefaultValue( "" )]
        [Browsable( true )]
        [Localizable( true )]
        [Category( "Folder Browsing" )]
        [Description( "The string that is displayed above the title of the dialog box." )]
        public string Title
        {
            get
            {
                Contract.Ensures( this.title != null );
                return this.title;
            }
            set
            {
                this.title = value ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the root folder where the browsing starts from.
        /// </summary>
        /// <value>One of the <see cref="T:System.Environment.SpecialFolder" /> values. The default is <see cref="T:System.Environment.SpecialFolder.Desktop" />.</value>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">The value assigned is not one of the <see cref="T:System.Environment.SpecialFolder" /> values. </exception>
        [Browsable( true )]
        [Localizable( false )]
        [DefaultValue( 0 )]
        [Category( "Folder Browsing" )]
        [Description( "The location of the root folder from which to start browsing. Only the specified folder and any subfolders that are beneath it will appear in the dialog box." )]
        [TypeConverter( "System.Windows.Forms.SpecialFolderEnumConverter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" )]
        public Environment.SpecialFolder RootFolder
        {
            get
            {
                return this.rootFolder;
            }
            set
            {
                if ( !Enum.IsDefined( typeof( Environment.SpecialFolder ), value ) )
                    throw new InvalidEnumArgumentException( "value", (int) value, typeof( Environment.SpecialFolder ) );

                this.rootFolder = value;
            }
        }

        /// <summary>
        /// Gets or sets the path selected by the user.
        /// </summary>
        /// <value>The path of the folder first selected in the dialog box or the last folder selected by the user.
        /// The default is an empty string ("").</value>
        [DefaultValue( "" )]
        [Browsable( true )]
        [Localizable( true )]
        [Category( "CatFolderBrowsing" )]
        [Description( "FolderBrowserDialogSelectedPath" )]
        [Editor( "System.Windows.Forms.Design.SelectedPathEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" )]
        public string SelectedPath
        {
            get
            {
                Contract.Ensures( Contract.Result<string>() != null );
                return this.selectedPath;
            }
            set
            {
                Contract.Requires<ArgumentNullException>( value != null, "value" );
                this.selectedPath = value;
            }
        }

        private static IShellItem GetFolder( Environment.SpecialFolder rootFolder, string selectedPath )
        {
            Contract.Requires( selectedPath != null );
            Contract.Ensures( Contract.Result<IShellItem>() != null );

            // use root folder is no path is selected
            if ( selectedPath.Length == 0 )
                selectedPath = Environment.GetFolderPath( rootFolder );

            // get folder (in case it's a file)
            selectedPath = Path.GetDirectoryName( selectedPath );

            // failover to root folder
            if ( !Directory.Exists( selectedPath ) )
                selectedPath = Path.GetDirectoryName( Environment.GetFolderPath( rootFolder ) );

            return NativeMethods.CreateItemFromParsingName( selectedPath );
        }

        private static string GetSelectedPath( IFileOpenDialog fileDialog )
        {
            string path;
            IShellItem item;

            fileDialog.GetResult( out item );
            item.GetDisplayName( ShellItemDisplayName.FileSystemPath, out path );
            Marshal.FinalReleaseComObject( item );

            return path;
        }

        private static IFileOpenDialog CreateDialog()
        {
            Contract.Ensures( Contract.Result<IFileOpenDialog>() != null );

            var options = FileDialogOptions.PickFolders | FileDialogOptions.ForceFileSystem | FileDialogOptions.PathMustExist;
            var dialog = new IFileOpenDialog();
            dialog.SetOptions( options );

            return dialog;
        }

        /// <summary>
        /// Resets the properties of a common dialog to their default values.
        /// </summary>
        public override void Reset()
        {
            this.Title = string.Empty;
            this.SelectedPath = string.Empty;
            this.RootFolder = Environment.SpecialFolder.Desktop;
        }

        /// <summary>
        /// Determines whether sufficient permissions for displaying a dialog exist.
        /// </summary>
        /// <remarks>The base implementation demands the <see cref="T:FileIOPermissionAccess.PathDiscovery">path discovery</see>
        /// <see cref="FileIOPermission">permission</see>.</remarks>
        protected override void CheckPermissionsToShowDialog()
        {
            var path = this.SelectedPath;

            if ( string.IsNullOrEmpty( path ) )
                path = Environment.GetFolderPath( this.RootFolder );

            new FileIOPermission( FileIOPermissionAccess.PathDiscovery, path ).Demand();
        }

        /// <summary>
        /// Displays a folder dialog of Win32 common dialog
        /// </summary>
        /// <param name="hwndOwner"><see cref="IntPtr">Handle</see> to the window that owns the dialog box.</param>
        /// <returns>If the user clicks the OK button of the dialog that is displayed, true is returned; otherwise, false.</returns>
        protected override bool RunDialog( IntPtr hwndOwner )
        {
            IShellItem folder = null;
            IFileOpenDialog dialog = null;
            var result = false;

            try
            {
                // create shell item for folder and create dialog
                folder = GetFolder( this.RootFolder, this.SelectedPath );
                dialog = CreateDialog();

                // configure dialog
                dialog.SetTitle( this.Title );
                dialog.SetFolder( folder );
                dialog.SetFileName( string.Empty );

                // show the dialog
                var hresult = dialog.Show( hwndOwner );
                result = hresult == 0U;

                // update the selected path if the user didn't cancel
                if ( result )
                    this.SelectedPath = GetSelectedPath( dialog );
            }
            finally
            {
                // free resources allocated by COM
                if ( folder != null )
                    Marshal.FinalReleaseComObject( folder );

                if ( dialog != null )
                    Marshal.FinalReleaseComObject( dialog );
            }

            return result;
        }
    }
}
