using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CustomImageChange
{
    public class ImageViewModel : ViewModelBase
    {
        #region Fields

        FileImageList targetList = new FileImageList();
        FileImageList sourceList = new FileImageList();
        Point startPoint;
        Point endPoint;
        Point cropOption;
        Point expandOpion;
        bool isPush = false;

        RelayCommand _nextImageCommand;
        RelayCommand _previousImageCommand;
        RelayCommand _cropAllImageCommand;
        RelayCommand _restoreTransCommand;
        RelayCommand _applyCropImageCommand;
        RelayCommand _successCommand;
        RelayCommand _passCommand;
        RelayCommand _targetPreviousCommand;
        RelayCommand _selectTargetFolderCommand;
        RelayCommand _selectSourceFolderCommand;

        RelayCommand _mouseDownCommand;
        RelayCommand _mouseUpCommand;
        RelayCommand _mouseMoveCommand;
        RelayCommand _cropUpCommand;
        RelayCommand _cropDownCommand;
        RelayCommand _cropLeftCommand;
        RelayCommand _cropRightCommand;
        RelayCommand _expandUpCommand;
        RelayCommand _expandDownCommand;
        RelayCommand _expandLeftCommand;
        RelayCommand _expandRightCommand;


        #endregion

        #region Properties

        public string TargetStatus
        {
            get => targetList.GetStatus();
        }

        public string SourceStatus
        {
            get => sourceList.GetStatus();
        }
        
        public BitmapImage TargetImage
        {
            get => targetList.GetSelectedImage();     
        }

        public BitmapImage SourceImage
        {
            get => sourceList.GetSelectedImage();
        }

        public CroppedBitmap CropImage
        {
            get => SourceImage == null ||
                SourceImage.Width < (int)Math.Round(RectLeft) + (int)Math.Round(RectWidth) ||
                SourceImage.Height < (int)Math.Round(RectTop) + (int)Math.Round(RectHeight) ||
                (RectWidth == 0 || RectHeight == 0)
                ? null :
                new CroppedBitmap(SourceImage, new Int32Rect(
                (int)Math.Round(RectLeft),
                (int)Math.Round(RectTop),
                (int)Math.Round(RectWidth),
                (int)Math.Round(RectHeight)));
        }

        public int CropImageLeft
        {
            get => (int)Math.Round(RectLeft + cropOption.X);
        }

        public int CropImageTop
        {
            get => (int)Math.Round(RectTop + cropOption.Y);
        }

        public double SourceLeft
        {
            get => TargetImage == null || SourceImage == null ||
                (TargetImage.Width > TargetImage.Height && 
                TargetImage.Height + SourceImage.Height < App.Current.MainWindow.Height) ?
                0 : TargetImage.Width + 5;
        }

        public double SourceTop
        {
            get => TargetImage != null && SourceImage != null &&
                (TargetImage.Width > TargetImage.Height &&
                TargetImage.Height + SourceImage.Height < App.Current.MainWindow.Height) ?
                TargetImage.Height + 5 : 0;
        }

        public double RectWidth
        {
            get => Math.Abs(startPoint.X - endPoint.X) + expandOpion.X;
        }

        public double RectHeight
        {
            get => Math.Abs(startPoint.Y - endPoint.Y) + expandOpion.Y;
        }

        public double RectLeft
        {
            get => Math.Min(startPoint.X, endPoint.X);
        }

        public double RectTop
        {
            get => Math.Min(startPoint.Y, endPoint.Y);
        }

        #endregion
              
        #region Command
        public RelayCommand NextImageCommand
        {
            get => _nextImageCommand ??
                    (_nextImageCommand = new RelayCommand(OnChangeNextImage));
            set => Set(ref _nextImageCommand, value);
        }

        public RelayCommand PreviousImageCommand
        {
            get => _previousImageCommand ??
                    (_previousImageCommand = new RelayCommand(OnChangePreviousImage));
            set => Set(ref _previousImageCommand, value);
        }

        public RelayCommand CropAllImageCommand
        {
            get => _cropAllImageCommand ??
                   (_cropAllImageCommand = new RelayCommand(OnCropAllImage));
            set => Set(ref _cropAllImageCommand, value);
        }

        public RelayCommand RestoreTransCommand
        {
            get => _restoreTransCommand ??
                   (_restoreTransCommand = new RelayCommand(OnRestoreTransImage));
            set => Set(ref _restoreTransCommand, value);
        }
        
        public RelayCommand ApplyCropImageCommand 
        {
            get => _applyCropImageCommand ??
                   (_applyCropImageCommand = new RelayCommand(OnApplyCropImage));
            set => Set(ref _applyCropImageCommand, value);
        }

        public RelayCommand SuccessCommand
        {
            get => _successCommand ??
                    (_successCommand = new RelayCommand(OnSuccessOneImage));
            set => Set(ref _successCommand, value);
        }

        public RelayCommand PassCommand
        {
            get => _passCommand ??
                    (_passCommand = new RelayCommand(OnPassImage));
            set => Set(ref _passCommand, value);
        }

        public RelayCommand TargetPreviousCommand
        {
            get => _targetPreviousCommand ??
                    (_targetPreviousCommand = new RelayCommand(OnTargetPrevious));
            set => Set(ref _targetPreviousCommand, value);
        }
        
        public RelayCommand SelectTargetFolderCommand
        {
            get => _selectTargetFolderCommand ??
                    (_selectTargetFolderCommand = new RelayCommand(OnSelectTargetFolder));
            set => Set(ref _selectTargetFolderCommand, value);
        }

        public RelayCommand SelectSourceFolderCommand
        {
            get => _selectSourceFolderCommand ??
                    (_selectSourceFolderCommand = new RelayCommand(OnSelectSourceFolder));
            set => Set(ref _selectSourceFolderCommand, value);
        }

        public RelayCommand MouseDownCommand
        {
            get => _mouseDownCommand ??
                    (_mouseDownCommand = new RelayCommand(OnMouseDown));
            set => Set(ref _mouseDownCommand, value);
        }

        public RelayCommand MouseUpCommand
        {
            get => _mouseUpCommand ??
                    (_mouseUpCommand = new RelayCommand(OnMouseUp));
            set => Set(ref _mouseUpCommand, value);
        }

        public RelayCommand MouseMoveCommand
        {
            get => _mouseMoveCommand ??
                    (_mouseMoveCommand = new RelayCommand(OnMouseMove));
            set => Set(ref _mouseMoveCommand, value);
        }

        public RelayCommand CropUpCommand
        {
            get => _cropUpCommand ??
                    (_cropUpCommand = new RelayCommand(OnCropUpMove));
            set => Set(ref _cropUpCommand, value);
        }

        public RelayCommand CropDownCommand
        {
            get => _cropDownCommand ??
                    (_cropDownCommand = new RelayCommand(OnCropDownMove));
            set => Set(ref _cropDownCommand, value);
        }

        public RelayCommand CropLeftCommand
        {
            get => _cropLeftCommand ??
                    (_cropLeftCommand = new RelayCommand(OnCropLeftMove));
            set => Set(ref _cropLeftCommand, value);
        }

        public RelayCommand CropRightCommand
        {
            get => _cropRightCommand ??
                    (_cropRightCommand = new RelayCommand(OnCropRightMove));
            set => Set(ref _cropRightCommand, value);
        }

        public RelayCommand ExpandUpCommand
        {
            get => _expandUpCommand ??
                    (_expandUpCommand = new RelayCommand(OnExpandUp));
            set => Set(ref _expandUpCommand, value);
        }

        public RelayCommand ExpandDownCommand
        {
            get => _expandDownCommand ??
                    (_expandDownCommand = new RelayCommand(OnExpandDown));
            set => Set(ref _expandDownCommand, value);
        }

        public RelayCommand ExpandLeftCommand
        {
            get => _expandLeftCommand ??
                    (_expandLeftCommand = new RelayCommand(OnExpandLeft));
            set => Set(ref _expandLeftCommand, value);
        }

        public RelayCommand ExpandRightCommand
        {
            get => _expandRightCommand ??
                    (_expandRightCommand = new RelayCommand(OnExpandRight));
            set => Set(ref _expandRightCommand, value);
        }

        #endregion

        #region ExcuteCommand

        private void OnChangeNextImage(object param)
        {
            InitImage();
            //ShowRect();
            ShowCropImage();
            sourceList.SelectNextImage();

            RaisePropertyChanged("SourceLeft");
            RaisePropertyChanged("SourceTop");
            RaisePropertyChanged("SourceImage");
            RaisePropertyChanged("SourceStatus");
        }

        private void OnChangePreviousImage(object param)
        {
            InitImage();
            ShowCropImage();
            sourceList.SelectPreviousImage();

            RaisePropertyChanged("SourceLeft");
            RaisePropertyChanged("SourceTop");
            RaisePropertyChanged("SourceImage");
            RaisePropertyChanged("SourceStatus");
        }

        private void OnCropAllImage(object param)
        {
            InitImage();
            endPoint.X = SourceImage.Width;
            endPoint.Y = SourceImage.Height;
            ShowCropImage();
        }

        private void OnRestoreTransImage(object param)
        {
            targetList.RestoreTransparency();
            ShowCropImage();

            RaisePropertyChanged("TargetImage");
        }
        
        private void OnApplyCropImage(object param)
        {
            targetList.ApplyCropImage(CropImage,
                CropImageLeft,
                CropImageTop,
                (int)Math.Round(RectWidth),
                (int)Math.Round(RectHeight));

            RaisePropertyChanged("TargetImage");
        }

        private void OnSuccessOneImage(object param)
        {
            targetList.SaveImage2FIle();
            InitImage();
            ShowCropImage(); 
            sourceList.SelectNextImage();
            targetList.SelectNextImage();

            RaisePropertyChanged("SourceTop");
            RaisePropertyChanged("SourceLeft");
            RaisePropertyChanged("SourceImage");
            RaisePropertyChanged("SourceStatus");
            RaisePropertyChanged("TargetImage");
            RaisePropertyChanged("TargetStatus");

        }

        private void OnPassImage(object param)
        {
            InitImage();
            ShowCropImage();
            targetList.SelectNextImage();

            RaisePropertyChanged("SourceTop");
            RaisePropertyChanged("SourceLeft");
            RaisePropertyChanged("TargetImage");
            RaisePropertyChanged("TargetStatus");
        }

        private void OnTargetPrevious(object param)
        {
            InitImage();
            ShowCropImage();
            targetList.SelectPreviousImage();

            RaisePropertyChanged("SourceTop");
            RaisePropertyChanged("SourceLeft");
            RaisePropertyChanged("TargetImage");
            RaisePropertyChanged("TargetStatus");
        }        

        private void OnSelectTargetFolder(object param)
        {
            InitImage();
            ShowCropImage();
            targetList.SelectImageList();
            targetList.SelectImage();

            RaisePropertyChanged("SourceTop");
            RaisePropertyChanged("SourceLeft");
            RaisePropertyChanged("TargetImage");
            RaisePropertyChanged("TargetStatus");
        }

        private void OnSelectSourceFolder(object param)
        {
            InitImage();
            ShowCropImage();
            sourceList.SelectImageList();
            sourceList.SelectImage();

            RaisePropertyChanged("SourceTop");
            RaisePropertyChanged("SourceLeft");
            RaisePropertyChanged("SourceImage");
            RaisePropertyChanged("SourceStatus");
        }

        private void OnMouseDown(object param)
        {
            InitImage();
            startPoint = Mouse.GetPosition(param as System.Windows.Controls.Image);            
        }

        private void OnMouseUp(object param)
        {
            endPoint = Mouse.GetPosition(param as System.Windows.Controls.Image);
           // ShowRect();
            ShowCropImage();
        }

        private void OnMouseMove(object param)
        {
            if (isPush)
            {
                // endPoint = Mouse.GetPosition(param as System.Windows.Controls.Image);
                // RaisePropertyChanged("RectWidth");
                // RaisePropertyChanged("RectHeight");
                // RaisePropertyChanged("RectLeft");
                // RaisePropertyChanged("RectTop");
                // _targetStatus = "(" + Mouse.GetPosition(param as System.Windows.Controls.Image).X + ", " + Mouse.GetPosition(param as System.Windows.Controls.Image).Y + ")";
                //RaisePropertyChanged("TargetStatus");

            }
        }

        private void OnCropUpMove(object param)
        {
            cropOption.Y--;
            ShowCropImage();
        }

        private void OnCropDownMove(object param)
        {
            cropOption.Y++;
            ShowCropImage();
        }

        private void OnCropLeftMove(object param)
        {
            cropOption.X--;
            ShowCropImage();
        }

        private void OnCropRightMove(object param)
        {
            cropOption.X++;
            ShowCropImage();
        }

        private void OnExpandUp(object param)
        {
            if (RectHeight> 0)
                expandOpion.Y--;
            ShowCropImage();
        }

        private void OnExpandDown(object param)
        {
            if (RectTop + RectHeight < SourceImage.Height)
                expandOpion.Y++;
            ShowCropImage();
        }

        private void OnExpandLeft(object param)
        {
            if (RectWidth > 0)
                expandOpion.X--;
            ShowCropImage();
        }

        private void OnExpandRight(object param)
        {
            if (RectLeft + RectWidth < SourceImage.Width - 1)
                expandOpion.X++;
            ShowCropImage();
        }

        #endregion

        #region CanCommand

        #endregion

        #region Method

        private void InitImage() 
        {
            startPoint = new Point();
            endPoint = new Point();
            cropOption = new Point();
            expandOpion = new Point();
        }

        private void ShowCropImage()
        {
            RaisePropertyChanged("CropImageTop");
            RaisePropertyChanged("CropImageLeft");
            RaisePropertyChanged("CropImage");
        }

        #endregion
    }
}
