using Licenser.Models;
using Licenser.Services;
using System.Collections.ObjectModel;

namespace Licenser.Pages;

public partial class Home
{
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (!firstRender) return;

        var list = DataSourceService.Table<KeyApp>().ToList();
        KeyApps.Clear();
        foreach (var item in list)
        {
            KeyApps.Add(item);
        }
        StateHasChanged();
    }

    private ObservableCollection<KeyApp> KeyApps { get; } = [];

    private void OpenEdit(KeyApp app)
    {
        NavigationManager.NavigateTo($"/edit/{app.Id}");
    }

    private void OpenNew()
    {
        NavigationManager.NavigateTo("/edit");
    }
}
