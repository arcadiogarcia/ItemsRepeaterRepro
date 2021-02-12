using Microsoft.Toolkit.Uwp.UI.Animations.Expressions;
using System;
using System.Collections.ObjectModel;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Composition.Interactions;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ItemsRepeaterRepro
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IInteractionTrackerOwner
    {

        private ObservableCollection<int> Elements { get; set; } = new ObservableCollection<int>() { };

        private readonly Visual _rootContainer;
        private readonly Visual _panelContainer;
        private readonly Compositor _compositor;
        private readonly VisualInteractionSource _scrollInteractionSource;
        private readonly InteractionTracker _tracker;

        private readonly SpringVector3NaturalMotionAnimation _openAnimation;

        const float openSize = 100;


        public MainPage()
        {
            this.InitializeComponent();

            _rootContainer = ElementCompositionPreview.GetElementVisual(this);
            _panelContainer = ElementCompositionPreview.GetElementVisual(navBarPanel);
            _compositor = _rootContainer.Compositor;
            _scrollInteractionSource = InitializeScrollInteractionSource();
            _tracker = InitializeTracker();
            InitializeSnapAnimationModifiers();
            InitializePanAnimation();


            _openAnimation = _compositor.CreateSpringVector3Animation();
            _openAnimation.DampingRatio = 0.75f;
            _openAnimation.FinalValue = new Vector3(0, openSize, 0);
            _openAnimation.Period = TimeSpan.FromSeconds(0.05f);


            Elements.Add(0);
            Elements.Add(0);
            Elements.Add(0);

            Elements.Add(0);
            Elements.Add(0);
            Elements.Add(0);
        }

        private VisualInteractionSource InitializeScrollInteractionSource()
        {
            var interactionSource = VisualInteractionSource.Create(_rootContainer);
            interactionSource.ScaleSourceMode = InteractionSourceMode.Disabled;
            interactionSource.PositionXSourceMode = InteractionSourceMode.Disabled;
            interactionSource.PositionYSourceMode = InteractionSourceMode.EnabledWithInertia;
            interactionSource.ManipulationRedirectionMode = VisualInteractionSourceRedirectionMode.CapableTouchpadAndPointerWheel;
            interactionSource.IsPositionXRailsEnabled = false;
            interactionSource.IsPositionYRailsEnabled = false;
            return interactionSource;
        }

        private InteractionTracker InitializeTracker()
        {
            var tracker = InteractionTracker.CreateWithOwner(_compositor, this);
            tracker.MaxPosition = new Vector3(0, (float)navBarPanel.ActualHeight, 0);
            tracker.MinPosition = new Vector3(0, 0, 0);
            tracker.PositionInertiaDecayRate = new Vector3(0.9f, 0.9f, 0.9f);
            tracker.InteractionSources.Add(_scrollInteractionSource);
            return tracker;
        }


        private void InitializeSnapAnimationModifiers()
        {
            var trackerNode = _tracker.GetReference();

            ScalarNode closedPosition = 0;

            var snapClosed = InteractionTrackerInertiaRestingValue.Create(_compositor);
            snapClosed.SetCondition(trackerNode.NaturalRestingPosition.Y < openSize / 2);
            snapClosed.SetRestingValue(closedPosition);

            var snapOpen = InteractionTrackerInertiaRestingValue.Create(_compositor);
            snapOpen.SetCondition(trackerNode.NaturalRestingPosition.Y < openSize * 1.5f);
            snapOpen.SetRestingValue((ScalarNode)openSize);

            var modifiers = new InteractionTrackerInertiaRestingValue[] { snapClosed, snapOpen };
            _tracker.ConfigurePositionYInertiaModifiers(modifiers);
        }

        private void InitializePanAnimation()
        {
            var positionOffsetAnimation = -_tracker.GetReference().Position.Y;
            _panelContainer.StartAnimation("Offset.Y", positionOffsetAnimation + _panelContainer.GetReference().Size.Y);
        }
        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _tracker.TryUpdatePositionWithAnimation(_openAnimation);
        }

        private void Button2_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Elements.Add(0);
        }

        public void CustomAnimationStateEntered(InteractionTracker sender, InteractionTrackerCustomAnimationStateEnteredArgs args)
        {
        }

        public void IdleStateEntered(InteractionTracker sender, InteractionTrackerIdleStateEnteredArgs args)
        {
        }

        public void InertiaStateEntered(InteractionTracker sender, InteractionTrackerInertiaStateEnteredArgs args)
        {
        }

        public void InteractingStateEntered(InteractionTracker sender, InteractionTrackerInteractingStateEnteredArgs args)
        {
        }

        public void RequestIgnored(InteractionTracker sender, InteractionTrackerRequestIgnoredArgs args)
        {
        }

        public void ValuesChanged(InteractionTracker sender, InteractionTrackerValuesChangedArgs args)
        {
        }

        private void itemsRepeater_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            statusText.Text = $"Repeater size: {itemsRepeater.ActualWidth} x {itemsRepeater.ActualHeight}";
        }
    }
}
