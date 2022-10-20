using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TodoApi.Models;
using Xunit;

namespace TodoApi.IntegrationTest
{
    public class TodoApiIntegrationTest
    {
        [Fact]
        public async Task Test_GetAll()
        {

            using (var client = new TestClientProvider().client)
            {
                var response = await client.GetAsync("/api/TodoItems");
                response.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_AddTodoItem()
        {
            using (var client = new TestClientProvider().client)
            {
                var response = await client.PostAsync("/api/TodoItems"
                        , new StringContent(
                            JsonConvert.SerializeObject(new TodoItem(){ Name = "Walk integration dog", IsComplete = true, Id = "9e882920-78f2-4aaa-a14a-b062345bc991" }),
                            Encoding.UTF8,
                            "application/json"));

                response.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            }
        }

        //Using FluentAssertions
        [Fact]
        public async Task Test_GetAllUsingFluentAssertion()
        {

            using (var client = new TestClientProvider().client)
            {
                var response = await client.GetAsync("/api/TodoItems");
                response.EnsureSuccessStatusCode();
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }

        //Using FluentAssertions
        [Fact]
        public async Task Test_AddTodoItemUsingFluentAssertion()
        {
            using (var client = new TestClientProvider().client)
            {
                var response = await client.PostAsync("/api/TodoItems"
                        , new StringContent(
                            JsonConvert.SerializeObject(new TodoItem() { Name = "Walk integration dog", IsComplete = true, Id = "9e882920-78f2-4aaa-a14a-b062345bc991" }),
                            Encoding.UTF8,
                            "application/json"));

                response.EnsureSuccessStatusCode();
                response.StatusCode.Should().Be(HttpStatusCode.Created);
            }
        }
    }
}