Mandro.Utils
============

Collection of utilities class that are too small for seperate project

## OwinService

Get started:

    var service = new OwinService();
    
    ...
    
    private void Configuration(IAppBuilder app)
    {
        app.UseOwinService(service);
    }
    
Handler returning file content:

    service.Get["/"].With(_ => File.ReadAllText(@"View\index.html"));
    
Handler with additional argument:

    service.Delete["/notice/(.+)"].With(
      async (_, noticeId) =>
      {
        repository.MarkAsReceived(noticeId);
        return string.Empty;
      });
